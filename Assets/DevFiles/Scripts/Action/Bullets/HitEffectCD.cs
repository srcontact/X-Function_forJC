using clrev01.Bases;
using Sirenix.OdinInspector;
using UnityEngine;

namespace clrev01.ClAction.Bullets
{
    [CreateAssetMenu(menuName = "CommonData/HitEffectCD")]
    public class HitEffectCD : CommonData<HitEffectCD, HitEffectLD, HitEffectHD>
    {
        protected override string parentName => "Effects";
        public int durationFrame = 1;
        public float baseRadius = 1;
        public AnimationCurve sizeCurve;
        public float curveLength;
        public bool IsExplosionHit;
        [BoxGroup("ExplosionPower"), ShowIf("IsExplosionHit")]
        public PowerPar ExplosionPower = new();
        protected override bool reuseLd => true;


        protected override void ResetLd(HitEffectLD ld)
        {
            base.ResetLd(ld);
            for (var i = 0; i < ld.alreadyHits.Length; i++)
            {
                ld.alreadyHits[i] = null;
            }
            ld.RunBeforePhysics();
        }

        public void OnValidate()
        {
            if (sizeCurve.length > 0)
            {
                curveLength = sizeCurve.keys[sizeCurve.length - 1].time;
            }
        }
    }
}