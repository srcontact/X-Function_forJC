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
    public class ArmorTypeData : HubData, IInfoTextData, IWeightSetting
    {
        [SerializeField]
        string name;
        public string Name => name;
        [field: SerializeField]
        public LocalizedString description { get; set; }

        public int minThickness = 1;
        public int maxThickness = 10;

        public float penetrationAttenuationRate = 0.1f;
        public float impactAttenuationRate = 0.1f;
        public float heatAttenuationRate = 0.1f;

        public float radiationRate = 0.97f;

        public float jammingSizeEffect = 0;

        public float weight = 1000;


        public void GetParameterText(ref Utf8ValueStringBuilder sb)
        {
            sb.AppendLine($"{hardwareCustom_armor.thicknessRange.SpaceToNbSp()} {minThickness * 10}mm\u00a0~\u00a0{maxThickness * 10}mm".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_armor.penetrationAttenuationRate.SpaceToNbSp()} {penetrationAttenuationRate}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_armor.impactAttenuationRate.SpaceToNbSp()} {impactAttenuationRate}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_armor.heatAttenuationRate.SpaceToNbSp()} {heatAttenuationRate}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_armor.radiationRate.SpaceToNbSp()} {radiationRate}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_armor.jamingSizeEffect.SpaceToNbSp()} {jammingSizeEffect} m".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_armor.unitWeight.SpaceToNbSp()} {weight} kg".Tagging("align", "flush").Tagging("u"));
        }

        public void GetParameterText(ref Utf8ValueStringBuilder sb, int armorThickness)
        {
            sb.AppendLine($"{hardwareCustom_armor.thicknessRange.SpaceToNbSp()} {minThickness * 10}mm\u00a0~\u00a0{maxThickness * 10}mm".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_armor.penetrationAttenuationRate.SpaceToNbSp()} {penetrationAttenuationRate * armorThickness:0.##}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_armor.impactAttenuationRate.SpaceToNbSp()} {impactAttenuationRate * armorThickness:0.##}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_armor.heatAttenuationRate.SpaceToNbSp()} {heatAttenuationRate * armorThickness:0.##}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_armor.radiationRate.SpaceToNbSp()} {radiationRate * armorThickness:0.##}".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_armor.jamingSizeEffect.SpaceToNbSp()} {jammingSizeEffect} m".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_armor.unitWeight.SpaceToNbSp()} {weight} kg".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_armor.grossWeight.SpaceToNbSp()} {GetWeightValue(armorThickness)} kg".Tagging("align", "flush").Tagging("u"));
        }

        private int CalcDamage(int rawDamage, int armorThickness, float attenuationRate)
        {
            return (int)(rawDamage / (1 + attenuationRate * armorThickness));
        }

        public int CalcPenetrationDamage(int rawDamage, int armorThickness)
        {
            return CalcDamage(rawDamage, armorThickness, penetrationAttenuationRate);
        }

        public int CalcImpactDamage(int rawDamage, int armorThickness)
        {
            return CalcDamage(rawDamage, armorThickness, impactAttenuationRate);
        }

        public int CalcHeatDamage(int rawDamage, int armorThickness)
        {
            return CalcDamage(rawDamage, armorThickness, heatAttenuationRate);
        }

        public float GetWeightValue(int ammoNum = 0)
        {
            return weight * ammoNum;
        }
    }
}