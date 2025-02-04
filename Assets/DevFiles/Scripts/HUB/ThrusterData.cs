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
    public class ThrusterData : HubData, IInfoTextData, IWeightSetting
    {
        [SerializeField]
        string name;
        public string Name => name;
        [field: SerializeField]
        public LocalizedString description { get; set; }

        public float maxPower = 600;
        public float thrustUseEnergy = 30;
        public float thrustHeat = 10;

        public float quickThrustPower = 6000;
        public float quickThrustUseEnergy = 300;
        public float quickThrustHeat = 100;
        public int quickThrustFrame = 3;
        public int quickThrustInterval = 3;

        public float weight = 500;


        public void GetParameterText(ref Utf8ValueStringBuilder sb)
        {
            sb.AppendLine($"{hardwareCustom_thruster.maxPower} {maxPower:0.##}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_thruster.useEnergyAtThrust} {thrustUseEnergy:0.##}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_thruster.heatAtThrust} {thrustHeat:0.##}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_thruster.quickThrustPower} {quickThrustPower:0.##}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_thruster.quickThrustUseEnergy} {quickThrustUseEnergy:0.##}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_thruster.quickThrustHeat} {quickThrustHeat:0.##}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_thruster.quickThrustFrame} {quickThrustFrame}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_thruster.Weight} {weight} kg".Tagging("align", "flush").Tagging("u"));
        }

        public float GetWeightValue(int ammoNum = 0)
        {
            return weight;
        }
    }
}