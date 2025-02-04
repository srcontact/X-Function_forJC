using static I2.Loc.ScriptLocalization;
using clrev01.Bases;
using clrev01.ClAction.Bullets;
using clrev01.ClAction.Machines;
using clrev01.ClAction.ObjectSearch;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using System;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class FireFuncPar : ActionFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.fire;
        public int fireWeapon;
        public VariableDataNumericGet numberOfContinuousShotsV = new() { constValue = 1 };
        public VariableDataNumericGet intervalV = new() { constValue = 1 };
        public WaitMode waitMode;
        public VariableDataNumericGet waitBeforeShootV = new() { constValue = 30 };
        public PrioritizeTgtMode prioritizeAimTgt = PrioritizeTgtMode.PrioritizeAimTgt;
        public VariableDataLockOnGet fireTgtList = new();
        public TargetingType targetingType = TargetingType.AimLockOnTarget;
        public VariableDataVector3Get aimPositionV = new();
        public VariableDataNumericGet aimHorizontalAngleV = new();
        public VariableDataNumericGet aimVerticalAngleV = new();


        private int _endNumberOfShots;
        [MemoryPackIgnore]
        public int latestFireInitFrame { get; private set; } = int.MinValue;
        [NonSerialized]
        [MemoryPackIgnore]
        public ObjectSearchTgt fireTgtObj;

        public enum PrioritizeTgtMode
        {
            PrioritizeAimTgt,
            PrioritizeFireTgt,
        }

        public enum WaitMode
        {
            Auto,
            Frame,
        }


        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (int* fw = &fireWeapon)
            fixed (WaitMode* wm = &waitMode)
            fixed (TargetingType* tt = &targetingType)
            fixed (PrioritizeTgtMode* pat = &prioritizeAimTgt)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_fireFuncPar.weapon, pgNodeParDescription_fireFuncPar.weapon);
                pgbepManager.SetPgbepSelectOptions(fw, pgbepManager.GetEquipmentList());
                pgbepManager.SetHeaderText(pgNodeParameter_fireFuncPar.fireTarget, pgNodeParDescription_fireFuncPar.fireTarget);
                pgbepManager.SetPgbepEnum(typeof(TargetingType), (int*)tt);
                switch (targetingType)
                {
                    case TargetingType.None:
                        break;
                    case TargetingType.AimLockOnTarget:
                        //todo:PrioritizeTgtMode(優先ターゲットモード)を仮に丸ごとEnumの値にすることで説明を省いているが、絶対に分かりづらいのでPgbepの方に説明を表示できるようにしてそちらで表示するようにしたい。
                        pgbepManager.SetPgbepEnum(typeof(PrioritizeTgtMode), (int*)pat);
                        fireTgtList.IndicateWithIndex(pgbepManager);
                        break;
                    case TargetingType.AimWithCoordinates:
                        aimPositionV.IndicateSwitchable(pgbepManager);
                        break;
                    case TargetingType.AimWithAngle:
                        pgbepManager.SetPgbepAngle(hAngleLimit: null, vAngleLimit: null, hVd: aimHorizontalAngleV, vVd: aimVerticalAngleV);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                pgbepManager.SetHeaderText(pgNodeParameter_fireFuncPar.numberOfFirings, pgNodeParDescription_fireFuncPar.numberOfFirings);
                numberOfContinuousShotsV.IndicateSwitchableInt(pgbepManager);
                pgbepManager.SetHeaderText(pgNodeParameter_fireFuncPar.interval, pgNodeParDescription_fireFuncPar.interval);
                var bcd = WHUB.GetBulletCD(StaticInfo.Inst.nowEditMech.mechCustom.weapons[fireWeapon]);
                var minInterval = bcd != null ? bcd.MinimumFiringInterval : 1;
                intervalV.IndicateSwitchableInt(pgbepManager);
                pgbepManager.SetHeaderText(pgNodeParameter_fireFuncPar.waitMode, pgNodeParDescription_fireFuncPar.waitMode);
                pgbepManager.SetPgbepEnum(typeof(WaitMode), (int*)wm);
                pgbepManager.SetHeaderText(pgNodeParameter_fireFuncPar.waitFrame, pgNodeParDescription_fireFuncPar.waitFrame);
                waitBeforeShootV.IndicateSwitchableInt(pgbepManager);
            }
        }

        public override bool CheckIsExecutable(MachineLD ld)
        {
            //残弾数を確認して0より大きい場合は射撃動作できる
            return ld.customData.mechCustom.weaponAmoNum[fireWeapon] - ld.runningShootHolder[fireWeapon].numberOfShots > 0;
        }

        public override void InitOnExecute(MachineLD ld)
        {
            base.InitOnExecute(ld);
            _endNumberOfShots = ld.runningShootHolder[fireWeapon].numberOfShots + numberOfContinuousShotsV.GetUseValueInt(ld, 0);
            fireTgtObj = targetingType is TargetingType.AimLockOnTarget ? fireTgtList.GetUseValue(ld) : null;
            latestFireInitFrame = ACM.actionFrame;
        }

        public override bool ActionExecute(MachineLD ld)
        {
            if (fireTgtObj != null && !fireTgtObj.gameObject.activeSelf) fireTgtObj = null;
            return false;
        }

        public override void OnEndAction(MachineLD ld)
        {
            base.OnEndAction(ld);
            fireTgtObj = null;
        }

        public override bool CheckEnd(MachineLD ld)
        {
            return _endNumberOfShots <= ld.runningShootHolder[fireWeapon].numberOfShots ||
                   ld.customData.mechCustom.weaponAmoNum[fireWeapon] - ld.runningShootHolder[fireWeapon].numberOfShots <= 0;
        }

        public override bool ActionOverwritable(ActionFuncPar other, MachineLD ld)
        {
            switch (other)
            {
                case FireFuncPar s:
                    return fireWeapon == s.fireWeapon;
                case StopActionFuncPar s:
                    return s.stopActionType switch
                    {
                        StopActionType.All => true,
                        StopActionType.Fire when (s.weaponFlags & 1L << fireWeapon) != 0 => true,
                        _ => false
                    };
                case FightFuncPar:
                case DefenceFuncPar:
                    return true;
                default:
                    return false;
            }
        }

        public override float EnergyCost(MachineLD ld)
        {
            // Fireノードは実行時ではなく、射撃時にENを消費する
            // 未実装だが、Aim時にもEN消費するようにすべきか？
            return 0;
        }

        public override float HeatCost(MachineLD ld)
        {
            return 0;
        }

        public override int? GetNodeFaceWeaponIcon()
        {
            return StaticInfo.Inst.nowEditMech.mechCustom.weapons[fireWeapon];
        }

        public override string[] GetNodeFaceText()
        {
            var waitStr = waitMode is WaitMode.Auto ? "auto" : $"{waitBeforeShootV.GetIndicateStr()}";
            var fireTgtStr = prioritizeAimTgt is PrioritizeTgtMode.PrioritizeAimTgt ? "A>F" : "F>A";
            var lockOnVariableStr = fireTgtList is not null ? $"{fireTgtList.GetIndicateStr()}" : null;
            return new[]
            {
                $"{fireWeapon}",
                numberOfContinuousShotsV.GetIndicateStr(),
                intervalV.GetIndicateStr(),
                waitStr,
                fireTgtStr,
                lockOnVariableStr
            };
        }
    }
}