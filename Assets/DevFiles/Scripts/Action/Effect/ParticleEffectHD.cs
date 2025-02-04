using clrev01.Bases;
using UnityEngine;

namespace clrev01.ClAction.Effect
{
    public class ParticleEffectHD : Hard<ParticleEffectCD, ParticleEffectLD, ParticleEffectHD>
    {
        [SerializeField]
        private VfxControl vfxControl;
        public override void RunBeforePhysics()
        {
            base.RunBeforePhysics();
            vfxControl.VfxPlay();
            if (ld.ExeFrameCount > ld.cd.fixedExitFrame)
            {
                gameObject.SetActive(false);
            }
        }
        public override void OnDotonExe()
        { }
    }
}