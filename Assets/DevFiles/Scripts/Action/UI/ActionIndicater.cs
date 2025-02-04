using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.Menu;
using clrev01.Save;
using Cysharp.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnumLocalizationWithI2Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static clrev01.Bases.UtlOfCL;
using static I2.Loc.ScriptLocalization;

namespace clrev01.ClAction.UI
{
    public class ActionIndicater : BaseOfCL, IRunner
    {
        private MachineHD nowSelectedMachineHD => ACM.machineList[ACM.cameraTgtMachine];
        [SerializeField]
        private StatusBar hpBar, shieldBar, heatBar, energyBar, stabilityBar;
        [SerializeField]
        private SightingInfos sightingInfos;
        [SerializeField]
        private TextMeshProUGUI teamNameHolder, machineNameHolder;
        [SerializeField]
        private TextMeshProUGUI moveStateHolder;
        [SerializeField]
        private TextMeshProUGUI actionStateHolder;
        [SerializeField]
        private MenuButton speedMeterButton, pauseButton;
        [SerializeField]
        private Text timeInd, gameSpeed;
        [SerializeField]
        private TextMeshProUGUI speedMeter, pauseText;
        public Slider speedMode;
        [SerializeField]
        private Toggle cameraObstacleAvoidanceToggle;
        [SerializeField]
        private TextMeshProUGUI variableInd;
        [SerializeField]
        private List<ActionEquipmentPanel> actionEquipmentPanels = new();
        [SerializeField]
        private List<ActionEquipmentPanel> actionOptionalsPanels = new();

        private SpeedUnitType actionUiSpeedUnitType
        {
            get => StaticInfo.Inst.ActionSettingData.actionUiSpeedUnitType;
            set => StaticInfo.Inst.ActionSettingData.actionUiSpeedUnitType = value;
        }


        private void Awake()
        {
            speedMeterButton.OnClick.AddListener(() =>
            {
                var speedUnitTypes = ((SpeedUnitType[])Enum.GetValues(typeof(SpeedUnitType))).ToList();
                var i = speedUnitTypes.IndexOf(actionUiSpeedUnitType);
                actionUiSpeedUnitType = speedUnitTypes[(i + 1) % speedUnitTypes.Count];
            });
            pauseButton.OnClick.AddListener((() =>
            {
                ACM.PauseChange();
                pauseText.text = ACM.pauseOnOff ? "Resume" : "Pause";
            }));
            var frameParSecs = ((FrameParSec[])Enum.GetValues(typeof(FrameParSec))).ToList();
            speedMode.minValue = 0;
            speedMode.maxValue = frameParSecs.Count - 1;
            speedMode.value = frameParSecs.IndexOf(ACM.frameParSec);
            speedMode.onValueChanged.AddListener(x => ChangeSpeedMode(x));
            cameraObstacleAvoidanceToggle.SetIsOnWithoutNotify(StaticInfo.Inst.ActionSettingData.cameraObstacleAvoidanceMode);
            cameraObstacleAvoidanceToggle.onValueChanged.AddListener(b => CameraObstacleAvoidanceMode(b));
        }
        public void RunBeforePhysics()
        { }
        public void RunAfterPhysics()
        { }
        public void RunOnUpdate()
        {
            SpeedMeterExe();
            MachineHealthExe();
            MachineVariableIndExe();
            MachineEquipmentIndExe();
            MachineOptionalsIndExe();
            MachineActionStateIndExe();
            SightInfosUpdate(nowSelectedMachineHD);
        }

        private void MachineHealthExe()
        {
            var ld = nowSelectedMachineHD.ld;
            var cd = ld.cd;
            var hp = cd.maxHearthPoint - ld.statePar.damage;
            hpBar.SetIndicate(cd.maxHearthPoint, hp);
            if (ld.shieldCd == null)
            {
                shieldBar.SetIndicate(10000, 0);
            }
            else
            {
                shieldBar.SetIndicate(ld.shieldCd.healthPoint, ld.shieldCd.healthPoint - ld.statePar.shieldDamage);
            }
            heatBar.SetIndicate(cd.allowableTemperature, ld.statePar.heat);
            energyBar.SetIndicate(ld.powerPlantData.energyCapacity, ld.powerPlantData.energyCapacity - ld.statePar.energyUsed);
            stabilityBar.SetIndicate(cd.baseStability, ld.statePar.impact);
        }

        private Utf8ValueStringBuilder sb = ZString.CreateUtf8StringBuilder();

        private void MachineVariableIndExe()
        {
            var vl = nowSelectedMachineHD.ld.indicateVariableList;
            sb.Clear();
            foreach (var vv in vl)
            {
                if (!vv.usedFlag) continue;
                sb.AppendLine($"{vv.Name}:{vv.ValueText}");
            }
            variableInd.text = sb.ToString();
        }
        private void MachineEquipmentIndExe()
        {
            for (int i = 0; i < actionEquipmentPanels.Count; i++)
            {
                var aep = actionEquipmentPanels[i];
                if (nowSelectedMachineHD.ld.customData.mechCustom.weapons.Count > i)
                {
                    var code = nowSelectedMachineHD.ld.customData.mechCustom.weapons[i];
                    var amount = nowSelectedMachineHD.ld.customData.mechCustom.weaponAmoNum[i] - nowSelectedMachineHD.ld.runningShootHolder[i].numberOfShots;
                    aep.SetIndicate(code != 0, (WHUB.GetBulletCD(code) as CommonDataBase)?.uiIcon, amount.ToString());
                }
                else aep.SetIndicate(false, null, "0");
            }
        }
        private void MachineOptionalsIndExe()
        {
            for (int i = 0; i < actionOptionalsPanels.Count; i++)
            {
                var aep = actionOptionalsPanels[i];
                if (nowSelectedMachineHD.ld.customData.mechCustom.optionParts.Count > i)
                {
                    var code = nowSelectedMachineHD.ld.customData.mechCustom.optionParts[i];
                    nowSelectedMachineHD.ld.statePar.optionPartsUseCount.TryGetValue(i, out var useCount);
                    var usableNum = i >= 0 && i < nowSelectedMachineHD.ld.customData.mechCustom.optionPartsUsableNum.Count ? nowSelectedMachineHD.ld.customData.mechCustom.optionPartsUsableNum[i] : 0;
                    var amount = usableNum - useCount;
                    var usingFrameStr = nowSelectedMachineHD.ld.statePar.optionPartsUseFrameDict.TryGetValue(i, out var usingFrame) && usingFrame - ACM.actionFrame > 0 ? $":{(usingFrame - ACM.actionFrame) / 60f:0.0}" : "";
                    var opData = OpHub.GetOptionPartsData(code);
                    aep.SetIndicate(code != 0, opData.uiIcon, $"{amount}{usingFrameStr}");
                }
                else aep.SetIndicate(false, null, "0");
            }
        }
        private void SpeedMeterExe()
        {
            if (ACM.machineList.Count <= 0 || nowSelectedMachineHD == null) return;
            var nowSpeed = nowSelectedMachineHD.ld.movePar.rBody.linearVelocity.magnitude * actionUiSpeedUnitType.GetSpeedUnitRatio();
            speedMeter.text = nowSpeed.ToString($"0.00 {actionUiSpeedUnitType.GetSpeedUnitText()}");
            timeInd.text = ((float)ACM.actionFrame / 60).ToString("0.00 sec");
            var gs = ((float)ACM.frameParSec) / 60;
            gameSpeed.text = gs.ToString("x0.00");
        }

        private void SightInfosUpdate(MachineHD hd)
        {
            sightingInfos.UpdateObjectContainer(ACM.machineList, nowSelectedMachineHD.pos, ACM.cameraTgtMachine);
            sightingInfos.UpdateSight(nowSelectedMachineHD.aimList, hd.transform);
        }
        public void ChangeMachineInfoIndicate()
        {
            teamNameHolder.text = $"Team{nowSelectedMachineHD.teamID.ToString("00")}-{nowSelectedMachineHD.machineIdInTeam.ToString("00")}";
            machineNameHolder.text = nowSelectedMachineHD.ld.customData.dataName;
        }
        private void MachineActionStateIndExe()
        {
            var ld = nowSelectedMachineHD.ld;
            moveStateHolder.text = $"{ld.movePar.moveState.ToLocalizedString()}{(ld.DuringLandingRigidity ? $"\n({action_actionInd.impactAbsorbing})" : "")}";
            var actions = new List<string>();
            if (ld.runningMoveTypeHolder.RunningAction != null)
            {
                actions.Add(ld.runningMoveTypeHolder.RunningAction.BlockTypeStr);
            }
            if (ld.runningRotateHolder.RunningAction != null)
            {
                actions.Add(ld.runningRotateHolder.RunningAction.BlockTypeStr);
            }
            if (ld.runningThrustHolder.RunningAction != null)
            {
                actions.Add(ld.runningThrustHolder.RunningAction.BlockTypeStr);
            }
            if (ld.runningFightHolder.RunningAction != null)
            {
                actions.Add(ld.runningFightHolder.RunningAction.BlockTypeStr);
            }
            if (ld.runningDefenceHolder.RunningAction != null)
            {
                actions.Add(ld.runningDefenceHolder.RunningAction.BlockTypeStr);
            }
            if (ld.runningShieldHolder.RunningAction != null)
            {
                actions.Add(ld.runningShieldHolder.RunningAction.BlockTypeStr);
            }
            if (ld.runningShootHolder.Any(x => x.RunningAction != null))
            {
                var shootList = new List<int>();
                for (var i = 0; i < ld.runningShootHolder.Count; i++)
                {
                    if (ld.runningShootHolder[i].RunningAction != null) shootList.Add(i + 1);
                }
                actions.Add($"{action_actionInd.fire}({ZString.Join(",", shootList)})");
            }
            actionStateHolder.text = actions.Count > 0 ? ZString.Join(" + ", actions) : action_actionInd.none;
        }
        private void ChangeSpeedMode(float speed)
        {
            var fps = ((FrameParSec[])Enum.GetValues(typeof(FrameParSec)))[(int)speed];
            ACM.frameParSec = fps;
        }
        private void CameraObstacleAvoidanceMode(bool arg0)
        {
            StaticInfo.Inst.ActionSettingData.cameraObstacleAvoidanceMode = arg0;
        }
    }
}