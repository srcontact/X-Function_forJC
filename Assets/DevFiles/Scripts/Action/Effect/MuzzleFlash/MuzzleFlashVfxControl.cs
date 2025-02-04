using UnityEngine;
using UnityEngine.VFX.Utility;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Effect.MuzzleFlash
{
    public class MuzzleFlashVfxControl : VfxControl
    {
        private readonly ExposedProperty _shootVelocity = "ShootVelocity";
        private readonly ExposedProperty _size = "Size";


        public void PlayVfx(Vector3 shootVelocity, float size)
        {
            vfx.Play();

            vfx.SetVector3(_shootVelocity, shootVelocity);
            vfx.SetFloat(_size, size);
        }

        public void UpdateVfx()
        {
            vfx.Simulate(1 / 60f, 1);
            stopFrame = CalcStopFrameOnVfxStop();
        }

        protected override int CalcStopFrameOnVfxStop()
        {
            return ACM.actionFrame + 60;
        }
    }
}