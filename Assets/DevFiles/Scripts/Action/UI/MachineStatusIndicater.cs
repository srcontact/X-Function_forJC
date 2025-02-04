using clrev01.Bases;
using clrev01.ClAction.Machines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.UI
{
    public class MachineStatusIndicater : BaseOfCL
    {
        public int teamNumInMatch = 0;
        public int machineNumInTeam = 0;
        MachineHD machineHd => ACM.machineList.Find(x => x.teamID == teamNumInMatch && x.machineIdInTeam == machineNumInTeam);
        [SerializeField]
        string hpStr = "HP : ";
        [SerializeField]
        TextMeshProUGUI nameText, hpPercent;
        [SerializeField]
        Image hpPercentBar;

        public void OnIndicate()
        {
            nameText.text = machineHd.ld.customData.dataName;
            float hpp = machineHd.ld.HpPercent;
            hpPercent.text = hpp.ToString(hpStr + "0.00%");
            hpPercentBar.fillAmount = hpp;
        }
    }
}