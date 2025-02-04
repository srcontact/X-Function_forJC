using System;
using System.Collections;
using clrev01.Bases;
using clrev01.Save;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace clrev01.Menu.BattleMenu.Arena
{
    [CreateAssetMenu(menuName = "Arena/ArenaBattleData")]
    public class ArenaBattleData : SOBaseOfCL
    {
        [SerializeField]
        private string dataName;
        [SerializeField]
        private LocalizedString title;
        public string titleStr => title != null ? title : dataName;
        [SerializeField, Multiline]
        private string dataDescription;
        [SerializeField]
        private LocalizedString description;
        public string descriptionStr => description != null ? description : dataDescription;

        public BattleRuleType battleRuleType;
        public bool fixedRandomSeed;
        public int randomSeed = 0;
        private ValueDropdownList<int> playLevelList => StaticInfo.Inst.actionLevelHub.playLevelValueDropdownList;
        [ValueDropdown("playLevelList")]
        public int playLevelNum;
        private ValueDropdownList<int> levelSizeList => StaticInfo.Inst.actionLevelHub.levelSizeValueDropdownList;
        [ValueDropdown("levelSizeList")]
        public int levelSizeNumber = 1;
        public int actionEndFrame = 120 * 60;

        [SerializeField]
        private List<BattlePerformanceMetrics> performanceMetrics = new();

        [SerializeField]
        private List<BattleTeamData> participatingTeamDatas = new();

        public MatchData GetMatchData(params TeamData[] playerTeams)
        {
            var teamDatas = new List<TeamData>();
            teamDatas.AddRange(playerTeams);
            foreach (var td in participatingTeamDatas)
            {
                teamDatas.Add(td.GetTeamData());
            }
            return new MatchData
            {
                fileName = titleStr,
                dataName = titleStr,
                teamList = teamDatas,
                fixedRandomSeed = fixedRandomSeed,
                randomSeed = randomSeed,
                playLevelNum = playLevelNum,
                levelSizeNumber = levelSizeNumber,
                actionEndFrame = actionEndFrame
            };
        }

        public string GetPerformanceMetricsText(ArenaBattleData battleData, BattleResultData resultData)
        {
            var hpRemain = (int)((resultData?.friendHpSum ?? 0) * 100);
            var timeRemain = battleData != null && resultData != null ? battleData.actionEndFrame - resultData.elapsedFrame / 60 : 0;
            var defeatEnemyNum = resultData?.defeatEnemyNum ?? 0;
            var res = "Battle Performance\n";
            foreach (var metric in performanceMetrics)
            {
                res += $" {metric.metricsType} : {metric.GetPerformanceLevel(hpRemain, timeRemain, defeatEnemyNum)}\n";
            }
            return res;
        }
    }

    [Serializable]
    public class BattlePerformanceMetrics
    {
        public BattlePerformanceMetricsType metricsType;
        [SerializeField]
        private List<int> performanceThresholds = new();

        public BattlePerformanceLevel GetPerformanceLevel(int hpRemain, int timeRemain, int defeatEnemyNum)
        {
            var battleData = metricsType switch
            {
                BattlePerformanceMetricsType.HpRemain => hpRemain,
                BattlePerformanceMetricsType.TimeRemain => timeRemain,
                BattlePerformanceMetricsType.DefeatEnemyNum => defeatEnemyNum,
                _ => throw new ArgumentOutOfRangeException()
            };

            for (var i = 0; i < performanceThresholds.Count; i++)
            {
                var threshold = performanceThresholds[i];
                if (threshold <= battleData) return (BattlePerformanceLevel)i;
            }

            return ((BattlePerformanceLevel[])Enum.GetValues(typeof(BattlePerformanceLevel)))[^1];
        }
    }

    public enum BattlePerformanceMetricsType
    {
        HpRemain,
        TimeRemain,
        DefeatEnemyNum,
    }

    public enum BattlePerformanceLevel
    {
        Gold = 0,
        Silver = 1,
        Bronze = 2,
    }
}