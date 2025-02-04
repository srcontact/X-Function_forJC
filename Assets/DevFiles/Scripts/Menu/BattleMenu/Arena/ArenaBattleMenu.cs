using System;
using System.Collections.Generic;
using clrev01.Bases;
using clrev01.Extensions;
using clrev01.Menu.CycleScroll;
using clrev01.Save;
using Cysharp.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Menu.BattleMenu.Arena
{
    public class ArenaBattleMenu : BaseOfCL, IDataTransport<SaveData>
    {
        public SaveData tData
        {
            get => playerTeamData;
            set => playerTeamData = (TeamData)value;
        }
        [SerializeField]
        private TextMeshProUGUI arenaDescriptionTxt;
        [SerializeField]
        private TextMeshProUGUI myTeamDescriptionTxt;
        [SerializeField]
        private TeamEditMenu.TeamEditManager teamEditManager;
        [SerializeField]
        private MenuButton playerTeamSelectButton;
        [SerializeField]
        private MenuButton startBattleButton;

        private ArenaBattleDataSet battleData => ArnHub.GetData(StaticInfo.Inst.selectedArena).arenaData.GetArenaBattle(StaticInfo.Inst.selectedArenaBattle);
        private BattleResultData resultData => StaticInfo.Inst.arenaSaveData.GetBattleResult(StaticInfo.Inst.selectedArena, StaticInfo.Inst.selectedArenaBattle);

        private TeamData playerTeamData
        {
            get => StaticInfo.Inst.arenaMyTeam;
            set => StaticInfo.Inst.arenaMyTeam = value;
        }

        private void Awake()
        {
            startBattleButton.OnClick.AddListener(() => OnStartBattle());
            playerTeamSelectButton.OnClick.AddListener(() => OnSelectTeam());
        }
        private void OnEnable()
        {
            UpdateInfoText();
        }
        private void UpdateInfoText()
        {
            arenaDescriptionTxt.text =
                $"{battleData.battleData.titleStr}\n" +
                $"{battleData.battleData.descriptionStr}\n" +
                $"\n" +
                $"{resultData?.GetResultText(battleData.battleData.battleRuleType)}\n" +
                $"\n" +
                $"{battleData.battleData.GetPerformanceMetricsText(battleData.battleData, resultData)}";

            if (playerTeamData is { machineList: not null })
            {
                myTeamDescriptionTxt.text =
                    $"{playerTeamData.dataName}\n    {string.Join("\n    ", playerTeamData.machineList.ConvertAll(x => x?.dataName ?? ""))}";
            }
            else
            {
                myTeamDescriptionTxt.text = "Not Selected";
            }
        }
        private void OnStartBattle()
        {
            StaticInfo.Inst.PlayMatch = battleData.battleData.GetMatchData(playerTeamData);
            StaticInfo.Inst.battleExecutionData.SetBattleExeData(BattleModeType.Arena, battleData.battleData.battleRuleType, StaticInfo.Inst.selectedArena, StaticInfo.Inst.selectedArenaBattle);
        }
        private void OnSelectTeam()
        {
            teamEditManager.isTestDataEdit = false;
            teamEditManager.SetEditTgt(this);
        }
    }
}