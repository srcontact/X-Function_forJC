using System;
using BurstLinq;
using clrev01.Bases;
using clrev01.Save;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.UI
{
    public class MatchStatusIndicator : BaseOfCL
    {
        [SerializeField]
        List<TeamStatusIndicator> teamIndicators = new();

        private void OnEnable()
        {
            var teamGroups = ACM.machineList.GroupBy(x => x.teamID).OrderBy(x => x.Key);
            var numberOfTeam = teamGroups.Count();
            var hpPercentList = teamGroups.ToList().ConvertAll(x =>
            {
                float hpRemainingSum = x.Sum(y => y.ld.HpRemaining);
                float hpSum = x.Sum(y => y.ld.cd.maxHearthPoint);
                return (teamId: x.Key, hpPercent: hpRemainingSum / hpSum, winLose: false);
            }).GroupBy(x => x.hpPercent).OrderByDescending(x => x.Key);
            int winnerTeamId;
            if (hpPercentList.FirstOrDefault().Count() == numberOfTeam) winnerTeamId = -1;
            else winnerTeamId = hpPercentList.FirstOrDefault().Min(x => x.teamId);
            for (int i = 0; i < teamIndicators.Count; i++)
            {
                teamIndicators[i].teamNumInMatch = i;
                if (winnerTeamId != -1) teamIndicators[i].winLose = i == winnerTeamId;
                teamIndicators[i].OnIndicate();
            }
        }

        private void OnDisable()
        {
            var teamGroups = ACM.machineList.GroupBy(x => x.teamID).OrderBy(x => x.Key);
            var numberOfTeam = teamGroups.Count();
            var exeData = StaticInfo.Inst.battleExecutionData;
            exeData.SaveBattleResult();
        }
    }
}