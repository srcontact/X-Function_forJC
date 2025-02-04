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
    public class CpuData : HubData, IInfoTextData, IWeightSetting
    {
        [SerializeField]
        string name;
        public string Name => name;
        [field: SerializeField]
        public LocalizedString description { get; set; }
        public int exeResourceSupply = 100;
        public float useEnergyPerFrame = 0.1f;
        public float weight = 10;

        public void GetParameterText(ref Utf8ValueStringBuilder sb)
        {
            sb.AppendLine($"{hardwareCustom_cpu.calcResourcePerFrame.SpaceToNbSp()} {exeResourceSupply}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_cpu.useEnergyPerFrame.SpaceToNbSp()} {useEnergyPerFrame}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_cpu.Weight.SpaceToNbSp()} {GetWeightValue()} kg".Tagging("align", "flush").Tagging("u"));
        }

        public float GetWeightValue(int ammoNum = 0)
        {
            return weight;
        }
    }
}