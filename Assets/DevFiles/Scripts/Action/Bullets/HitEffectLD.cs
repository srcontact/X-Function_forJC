using clrev01.Bases;
using UnityEngine;

namespace clrev01.ClAction.Bullets
{
    public class HitEffectLD : LocalData<HitEffectCD, HitEffectLD, HitEffectHD>
    {
        public Collider[] alreadyHits = new Collider[20];

        public override void RunBeforePhysics()
        {
            base.RunBeforePhysics();
            if (ExeFrameCount > cd.durationFrame)
            {
                hd.scl = Vector3.one * (cd.baseRadius * 2);
                hd.Disable();
                return;
            }
            hd.scl = Vector3.one * (GetRadius() * 2);
        }
        public float GetRadius()
        {
            if (cd.durationFrame <= 1)
            {
                return cd.baseRadius;
            }
            var e = (float)ExeFrameCount % (cd.durationFrame + 1) / cd.durationFrame;
            return cd.sizeCurve.Evaluate(e * cd.curveLength) * cd.baseRadius;
        }
    }
}