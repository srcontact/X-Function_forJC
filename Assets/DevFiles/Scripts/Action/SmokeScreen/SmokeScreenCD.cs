using clrev01.Bases;
using clrev01.Extensions;
using Cysharp.Text;
using UnityEngine;
using UnityEngine.Serialization;
using static I2.Loc.ScriptLocalization;

namespace clrev01.ClAction.SmokeScreen
{
    [CreateAssetMenu(menuName = "CommonData/SmokeScreen")]
    public class SmokeScreenCD : CommonData<SmokeScreenCD, SmokeScreenLD, SmokeScreenHD>
    {
        public int effectiveFrame = 180;
        public int residualFrame = 120;
        public int lifeFrame => effectiveFrame + residualFrame;
        public AnimationCurve sizeCurvePerFrame;
        public float maxSize = 100;
        public float maxDepenetrationVelocity = 2;
        public float jammingSize = 300;

        public SmokeScreenHD SpawnSmokeScreen(HardBase generatorObj, Vector3 position, Quaternion rotation)
        {
            var hd = InstActor(position, rotation);
            hd.OnSpawn(generatorObj, maxDepenetrationVelocity);
            return hd;
        }
        public float GetSizePerFrame(int exeFrame)
        {
            return sizeCurvePerFrame.Evaluate(exeFrame / (float)effectiveFrame) * maxSize;
        }

        public void GetParameterText(ref Utf8ValueStringBuilder sb)
        {
            sb.AppendLine($"{hardwareCustom_opitonals_smokeScreen.effectiveFrame} {effectiveFrame / 60f:0.0} s".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_opitonals_smokeScreen.residualFrame} {residualFrame / 60f:0.0} s".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_opitonals_smokeScreen.maxSize} {maxSize} m".Tagging("align", "flush").Tagging("u"));
            sb.AppendLine($"{hardwareCustom_opitonals_smokeScreen.jammingSize} {jammingSize} m".Tagging("align", "flush").Tagging("u"));
        }
    }
}