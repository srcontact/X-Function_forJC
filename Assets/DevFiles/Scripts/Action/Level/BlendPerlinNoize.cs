using System.Collections.Generic;
using UnityEngine;

namespace clrev01.ClAction.Level
{
    [System.Serializable]
    public class BlendPerlinNoize : ExPerlinNoize
    {
        [SerializeField]
        AnimationCurve blendCurve = new AnimationCurve();
        [SerializeField]
        List<ValuePerlinNoize> noizesBiggerThreshold = new List<ValuePerlinNoize>();
        [SerializeField]
        List<ValuePerlinNoize> noizesSmallerThreshold = new List<ValuePerlinNoize>();
        [SerializeField]
        List<BlendPerlinNoize> childs = new List<BlendPerlinNoize>();

        public override float Value(float x, float y)
        {
            float v = base.Value(x, y);
            float b = blendCurve.Evaluate(v);
            float f = 0;
            foreach (var pn in noizesBiggerThreshold)
            {
                f += pn.Value(x, y) * (1f - b);
            }
            foreach (var pn in noizesSmallerThreshold)
            {
                f += pn.Value(x, y) * b;
            }
            foreach (var ch in childs)
            {
                f += ch.Value(x, y);
            }
            return f;
        }
    }
}