using UnityEngine;
using UnityEngine.VFX;

namespace clrev01.ClAction.Effect
{
    public class FixedStopVfxController : VfxControl
    {
        [SerializeField]
        private int fixedStopFrame = 600;

        public void EffectExe()
        {
            vfx.Play();
            vfx.Simulate(1f / 60f, 1);
            stopFrame = CalcStopFrameOnVfxStop();
        }

        protected override int CalcStopFrameOnVfxStop()
        {
            return fixedStopFrame + ActionManager.Inst.actionFrame;
        }
    }
}