using UnityEngine;
using UnityEngine.VFX.Utility;

namespace clrev01.ClAction.Effect.Smoke
{
    public class DamageSmokeVfxControl : VfxControl
    {
        [Range(0, 1)]
        public float smokeStartThreshold = 0.5f;
        [Range(0, 1)]
        public float fireOnThreshold = 0.9f;

        private readonly ExposedProperty _power = "Power";
        private readonly ExposedProperty _fireOn = "FireOn";
        private readonly ExposedProperty _spawnPos = "SpawnPosition";
        private readonly ExposedProperty _ownerVelocity = "OwnerVelocity";
        private readonly ExposedProperty _lifeTimeMax = "LifetimeMax";

        private int _lifeFrameMax;

        protected override void Start()
        {
            base.Start();
            _lifeFrameMax = (int)(vfx.GetFloat(_lifeTimeMax) * 60);
        }

        public void EffectExe(float effectPower, Vector3 spawnPos, Vector3 ownerVelocity)
        {
            var pow = effectPower < smokeStartThreshold ? 0 : (effectPower - smokeStartThreshold) / (1 - smokeStartThreshold);

            vfx.Play();

            vfx.SetFloat(_power, pow);
            vfx.SetBool(_fireOn, pow > fireOnThreshold);
            vfx.SetVector3(_spawnPos, spawnPos - ownerVelocity / 60);
            vfx.SetVector3(_ownerVelocity, ownerVelocity);

            vfx.Simulate(1f / 60f, 1);
            stopFrame = CalcStopFrameOnVfxStop();
        }

        protected override int CalcStopFrameOnVfxStop()
        {
            return ActionManager.Inst.actionFrame + _lifeFrameMax;
        }
    }
}