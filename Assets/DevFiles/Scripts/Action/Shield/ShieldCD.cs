using clrev01.Bases;
using clrev01.Extensions;
using clrev01.Save;
using Cysharp.Text;
using System.Text;
using clrev01.ClAction.Effect;
using I2.Loc;
using UnityEngine;
using static I2.Loc.ScriptLocalization;
using static I2.Loc.ScriptLocalization.hardwareCustom_shield;

namespace clrev01.ClAction.Shield
{
    [CreateAssetMenu(menuName = "CommonData/ShieldCD")]
    public class ShieldCD : SlaveObjectCD<ShieldCD, ShieldLD, ShieldHD>, IWeightSetting
    {
        protected override string parentName => "Shields";
        public int healthPoint = 10000;
        public float damageReduceRate = 0.2f;
        public int useDamageRateParFrame = 40;
        public int recoveryPointParFrame = 10;
        [Range(0f, 1f)]
        public float breakRecoveryRate = 0.25f;
        public float useEnergy = 5;
        public int reloadFrame = 2;
        public int recoveryStartFrame = 60;
        public float maxRadius = 10;
        public float maxCenterRange = 8;
        [ColorUsage(false, true)]
        public Color normalColor, onHitColor;
        public AnimationCurve onHitColorRatio = new() { keys = new[] { new Keyframe(0, 1), new Keyframe(5, 0) } };
        public int onPenetrateDamageToOpponent = 1;
        public int onPenetrateDamageToSelf = 10;
        public int onShieldToShieldPenetrateDamage = 100;

        public float weight = 500;

        public ParticleEffectCD onHitVfx;


        public (float radiusMin, float radiusMax, float offsetMin, float offsetMax) GetShieldMinMax(CoordinateSystemType coordinateSystemType)
        {
            var offsetRange = coordinateSystemType is CoordinateSystemType.Local ? maxCenterRange : 4000;
            return (0, maxRadius, -offsetRange, offsetRange);
        }

        public override void GetParameterText(ref Utf8ValueStringBuilder sb)
        {
            sb.AppendLine($"{hardwareCustom_shield.healthPoint} {healthPoint}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_shield.useEnergy} {useEnergy}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_shield.maxRadius} {maxRadius}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_shield.maxCenterRange} {maxCenterRange}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_shield.damageReduceRate} {damageReduceRate}".GetNormalInfoStr());
            sb.AppendLine($"{usageDamageRate} {useDamageRateParFrame}".GetNormalInfoStr());
            sb.AppendLine($"{recoveryRate} {recoveryPointParFrame}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_shield.recoveryStartFrame} {recoveryStartFrame}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_shield.reloadFrame} {reloadFrame}".GetNormalInfoStr());
            sb.AppendLine($"{availableAfterBreak} {breakRecoveryRate}".GetNormalInfoStr());
            sb.AppendLine($"{interferenceObjectDamage} {interferenceObjectDamage_obj}:{onPenetrateDamageToOpponent} {interferenceObjectDamage_shld}:{onPenetrateDamageToSelf}".GetNormalInfoStr());
            sb.AppendLine($"{interferenceShieldDamage} {onShieldToShieldPenetrateDamage}".GetNormalInfoStr());
            sb.AppendLine($"{hardwareCustom_shield.weight} {GetWeightValue()} kg".Tagging("align", "flush").Tagging("u"));
        }

        public float GetWeightValue(int ammoNum = 0)
        {
            return weight;
        }
    }
}