using UnityEngine.VFX.Utility;

namespace clrev01.ClAction.Effect.Smoke
{
    public class SmokeScreenVfxControl : VfxControl
    {
        private int _lifeFrame;
        private readonly ExposedProperty _alphaRate = "AlphaRate";


        public void Init(int lifeFrame)
        {
            _lifeFrame = lifeFrame;
        }
        public void EffectExe(float alphaRate)
        {
            vfx.Play();

            vfx.SetFloat(_alphaRate, alphaRate);

            vfx.Simulate(1f / 60f, 1);
            stopFrame = CalcStopFrameOnVfxStop();
        }
        protected override int CalcStopFrameOnVfxStop()
        {
            return ActionManager.Inst.actionFrame + _lifeFrame;
        }
    }
}