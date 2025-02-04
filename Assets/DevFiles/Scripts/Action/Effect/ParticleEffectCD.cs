using clrev01.Bases;
using UnityEngine;

namespace clrev01.ClAction.Effect
{
    [CreateAssetMenu(menuName = "CommonData/ParticleEffectCD")]
    public class ParticleEffectCD : CommonData<ParticleEffectCD, ParticleEffectLD, ParticleEffectHD>
    {
        public int fixedExitFrame = 180;

    }
}