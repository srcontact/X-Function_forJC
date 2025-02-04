using UnityEngine;
using UnityEngine.VFX.Utility;

namespace clrev01.ClAction.Effect.Thruster
{
    public class ThrusterTrailVfxController : VfxControl
    {
        private ExposedProperty power = "Power";
        private ExposedProperty _lifeTimeMax = "LifetimeMax";
        private Vector3? _currentPos;
        private Vector3? _currentVelocity;

        private int _lifeFrameMax;

        protected override void Start()
        {
            base.Start();
            _lifeFrameMax = (int)(vfx.GetFloat(_lifeTimeMax) * 60);
        }

        public void EffectExe(float effectPower)
        {
            vfx.Play();

            vfx.SetFloat(this.power, effectPower);
            vfx.SetVector3("NowSpawnPos", pos);
            vfx.SetVector3("CurrentSpawnPos", _currentPos ?? pos);
            _currentPos = pos;
            var velocity = Quaternion.LookRotation(transform.forward, Vector3.up).eulerAngles;
            vfx.SetVector3("NowVelocity", velocity);
            vfx.SetVector3("CurrentVelocity", _currentVelocity ?? velocity);
            _currentVelocity = velocity;

            vfx.Simulate(1f / 60f, 1);
            stopFrame = CalcStopFrameOnVfxStop();
        }

        protected override int CalcStopFrameOnVfxStop()
        {
            return ActionManager.Inst.actionFrame + _lifeFrameMax;
        }

        private void OnDisable()
        {
            _currentPos = null;
            _currentVelocity = null;
        }
    }
}