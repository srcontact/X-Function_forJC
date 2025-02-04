using BurstLinq;
using clrev01.Bases;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.UI
{
    public class TeamStatusIndicator : BaseOfCL
    {
        public int teamNumInMatch = 0;
        [SerializeField]
        List<MachineStatusIndicater> machineIndicators = new();
        [SerializeField]
        string hpSumStr = "HP Sum : ";
        [SerializeField]
        TextMeshProUGUI nameText, hpSumPercentText, winLoseText;
        [SerializeField]
        Image hpSumPercentBar;
        public bool? winLose { get; set; }

        public void OnIndicate()
        {
            for (int i = 0; i < machineIndicators.Count; i++)
            {
                if (!ACM.machineList.Any(x => x.teamID == teamNumInMatch && x.machineIdInTeam == i)) machineIndicators[i].gameObject.SetActive(false);
                else
                {
                    machineIndicators[i].teamNumInMatch = teamNumInMatch;
                    machineIndicators[i].machineNumInTeam = i;
                    machineIndicators[i].OnIndicate();
                }
            }

            var teamMachines = ACM.machineList.FindAll(x => x.teamID == teamNumInMatch);
            float hpRemainingSum = teamMachines.Sum(x => x.ld.HpRemaining);
            float hpSum = teamMachines.Sum(x => x.ld.cd.maxHearthPoint);
            hpSumPercentText.text = (hpRemainingSum / hpSum).ToString(hpSumStr + "0.00%");
            hpSumPercentBar.fillAmount = hpRemainingSum / hpSum;
            nameText.text = StaticInfo.Inst.PlayMatch.teamList[teamNumInMatch].dataName;
            if (winLose.HasValue) winLoseText.text = winLose.Value ? "WIN" : "LOSE";
            else winLoseText.text = "Draw";
        }
    }
}