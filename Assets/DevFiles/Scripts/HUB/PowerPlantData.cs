using clrev01.ClAction;
using clrev01.Extensions;
using clrev01.Menu.InformationIndicator;
using Cysharp.Text;
using I2.Loc;
using System.Text;
using UnityEngine;
using static I2.Loc.ScriptLocalization;

namespace clrev01.HUB
{
    [System.Serializable]
    public class PowerPlantData : HubData, IInfoTextData, IWeightSetting
    {
        [SerializeField]
        string name;
        public string Name => name;
        [field: SerializeField]
        public LocalizedString description { get; set; }

        public float energySupplyRate = 3;
        public int supplyRestartFrame = 30;
        public float energyCapacity = 1350;
        public float weight = 1000;


        public void GetParameterText(ref Utf8ValueStringBuilder sb)
        {
            sb.AppendLine($"{hardwareCustom_powerPlant.energySupplyRate.SpaceToNbSp()} {energySupplyRate}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_powerPlant.supplyRestartFrame.SpaceToNbSp()} {supplyRestartFrame}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_powerPlant.energyCapacity.SpaceToNbSp()} {energyCapacity}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_powerPlant.weight.SpaceToNbSp()} {GetWeightValue()} kg".Tagging("align", "flush").Tagging("u"));
        }

        public float GetWeightValue(int ammoNum = 0)
        {
            return weight;
        }
    }
}