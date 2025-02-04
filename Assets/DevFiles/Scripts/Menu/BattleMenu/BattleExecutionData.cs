using System;
using System.Collections.Generic;
using System.Linq;
using clrev01.Bases;
using clrev01.Menu.BattleMenu.Arena;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Menu.BattleMenu
{
    public class BattleExecutionData
    {
        public BattleModeType battleMode;
        public BattleRuleType battleRule;
        public int[] battleCodeList;
        public BattleResultData battleResultData;

        public void SetBattleExeData(BattleModeType battleModeType, BattleRuleType battleRuleType, params int[] battleCodes)
        {
            battleMode = battleModeType;
            battleRule = battleRuleType;
            battleCodeList = battleCodes;
        }

        public void InitResultData(int numberOfTeam)
        {
            battleResultData = new BattleResultData
            {
                numberOfDefeats = Enumerable.Repeat(0, numberOfTeam).ToList()
            };
        }

        public void AddNumberOfDefeat(int teamID)
        {
            battleResultData.numberOfDefeats[teamID] += 1;
        }

        public void SaveBattleResult()
        {
            battleResultData.elapsedFrame = ACM.actionFrame;

            var teamGroups = ACM.machineList.GroupBy(x => x.teamID).OrderBy(x => x.Key);
            var hpPercentList = teamGroups.ToList().ConvertAll(x =>
            {
                float hpRemainingSum = x.Sum(y => y.ld.HpRemaining);
                float hpSum = x.Sum(y => y.ld.cd.maxHearthPoint);
                return (teamId: x.Key, hpPercent: hpRemainingSum / hpSum);
            }).GroupBy(x => x.teamId).OrderBy(x => x.Key);
            foreach (var hpGroup in hpPercentList)
            {
                battleResultData.remainingHpData.Add(hpGroup.ToList().ConvertAll(x => x.hpPercent));
            }

            switch (battleMode)
            {
                case BattleModeType.Test:
                    break;
                case BattleModeType.Arena:
                    var asd = StaticInfo.Inst.arenaSaveData;
                    if (!asd.battleResults.TryGetValue(battleCodeList[0], out var arenaResult))
                    {
                        arenaResult = new Dictionary<int, BattleResultData>();
                        asd.battleResults.Add(battleCodeList[0], arenaResult);
                    }
                    if (!arenaResult.TryGetValue(battleCodeList[1], out var existingData))
                    {
                        asd.battleResults[battleCodeList[0]].Add(battleCodeList[1], battleResultData);
                    }
                    else
                    {
                        var friendHpSum = battleResultData.remainingHpData[0].Sum();
                        var enemyHpSum = battleResultData.remainingHpData.Sum(x => x.Sum()) - friendHpSum;
                        if ((!existingData.GetIsCleared(battleRule) && battleResultData.GetIsCleared(battleRule)) ||
                            existingData.numberOfDefeats[0] > battleResultData.numberOfDefeats[0] ||
                            existingData.numberOfDefeats.Sum() - existingData.numberOfDefeats[0] < battleResultData.numberOfDefeats.Sum() - battleResultData.numberOfDefeats[0] ||
                            existingData.remainingHpData[0].Sum() < friendHpSum ||
                            existingData.remainingHpData.Sum(x => x.Sum()) - existingData.remainingHpData[0].Sum() > enemyHpSum ||
                            existingData.elapsedFrame > battleResultData.elapsedFrame)
                        {
                            arenaResult[battleCodeList[1]] = battleResultData;
                        }
                    }

                    StaticInfo.Inst.SaveArenaSave();
                    break;
                case BattleModeType.Match:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum BattleModeType
    {
        Test,
        Arena,
        Match,
    }
}