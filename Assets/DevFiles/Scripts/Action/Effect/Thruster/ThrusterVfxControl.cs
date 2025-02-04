using System;
using UnityEngine;
using UnityEngine.VFX.Utility;

namespace clrev01.ClAction.Effect.Thruster
{
    public class ThrusterVfxControl : VfxControl
    {
        private readonly ExposedProperty _lifeFrame = "LifeFrame";
        private readonly ExposedProperty _positionCurve = "PositionCurve";
        private readonly ExposedProperty _sizeCurve = "SizeCurve";

        [SerializeField, Range(0f, 20f)]
        private float enginePower = 1;

        [SerializeField, Range(2, 5)]
        private int lifeFrameValue = 1;

        [SerializeField]
        private AnimationCurve positionCurveValue;
        [SerializeField, Range(0, 20)]
        private float posLength = 1;

        [SerializeField]
        private AnimationCurve sizeCurveValue;
        [SerializeField]
        private float size = 1;
        private Keyframe _posKey1;
        private Keyframe _sizeKey1;

        private void Awake()
        {
            _posKey1 = positionCurveValue.keys[1];
            _sizeKey1 = sizeCurveValue.keys[1];
        }
        public void ThrusterExe(float enginePower)
        {
            vfx.Play();

            this.enginePower = enginePower;

            vfx.SetInt(_lifeFrame, lifeFrameValue);

            _posKey1.value = posLength * this.enginePower;
            positionCurveValue.MoveKey(1, _posKey1);
            // positionCurveValue.SmoothTangents(1, 1);
            vfx.SetAnimationCurve(_positionCurve, positionCurveValue);

            _sizeKey1.value = size * this.enginePower;
            sizeCurveValue.MoveKey(1, _sizeKey1);
            vfx.SetAnimationCurve(_sizeCurve, sizeCurveValue);

            vfx.Simulate(1f / 60f, 1);
            stopFrame = CalcStopFrameOnVfxStop();
        }

        protected override int CalcStopFrameOnVfxStop()
        {
            return ActionManager.Inst.actionFrame + lifeFrameValue;
        }
    }
}