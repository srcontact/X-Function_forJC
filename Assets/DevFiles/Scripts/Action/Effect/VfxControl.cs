using clrev01.Bases;
using UnityEngine;
using UnityEngine.VFX;

namespace clrev01.ClAction.Effect
{
    public abstract class VfxControl : BaseOfCL
    {
        [SerializeField]
        protected VisualEffect vfx;
        public bool stillAliveParticle => stopFrame >= ActionManager.Inst.actionFrame;
        protected int stopFrame;
        protected bool vfxPlayNow;

        protected virtual void Start()
        {
            vfx.pause = true;
        }

        public void VfxPlay()
        {
            if (!vfxPlayNow)
            {
                vfxPlayNow = true;
                vfx.Play();
            }
            vfx.Simulate(1f / 60f, 1);
        }

        public void VfxStop()
        {
            vfxPlayNow = false;
            vfx.Stop();
            vfx.Simulate(1f / 60f, 1);
        }

        protected abstract int CalcStopFrameOnVfxStop();

        public void VfxStopImmediately()
        {
            vfxPlayNow = false;
            vfx.Reinit();
            stopFrame = ActionManager.Inst.actionFrame;
        }
    }
}