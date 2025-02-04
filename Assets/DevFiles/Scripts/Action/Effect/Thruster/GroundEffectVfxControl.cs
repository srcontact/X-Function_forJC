using UnityEngine;
using UnityEngine.VFX.Utility;

namespace clrev01.ClAction.Effect.Thruster
{
    public class GroundEffectVfxControl : VfxControl
    {
        private readonly string _constantSpawnEvent = "OnPlayConstant";
        private readonly string _burstSpawnEvent = "OnPlayBurst";
        private readonly ExposedProperty _spawnPos = "SpawnPos";
        private readonly ExposedProperty _effectRotate = "EffectRotate";
        private readonly ExposedProperty _power = "Power";
        private readonly ExposedProperty _lifetimeMax = "LifetimeMax";
        private readonly ExposedProperty _lifetimeMin = "LifetimeMin";
        private readonly ExposedProperty _spawnNum = "SpawnNum";
        private readonly ExposedProperty _outputYAngle = "OutputYAngle";
        private readonly ExposedProperty _outputYDirection = "OutputYAngle";
        private readonly ExposedProperty _outputZAngleMax = "OutputYAngle";
        private readonly ExposedProperty _outputZAngleMin = "OutputYAngle";
        private readonly ExposedProperty _outputMagnitudeMax = "OutputYAngle";
        private readonly ExposedProperty _outputMagnitudeMin = "OutputYAngle";
        private readonly ExposedProperty _spawnPosSize = "SpawnPosSize";
        private readonly ExposedProperty _initSpeedRatio = "InitSpeedRatio";

        public enum EffectMode
        {
            Constant,
            Burst,
        }

        public struct EffectPar
        {
            public EffectMode effectMode;
            public Vector3? spawnPos;
            public Vector3? effectRotate;
            public float? power;
            public float? spawnNum;
            public float? outputYAngle;
            public float? outputYDirection;
            public float? outputZAngleMax;
            public float? outputZAngleMin;
            public float? outputMagnitudeMax;
            public float? outputMagnitudeMin;
            public float? spawnPosSize;
            public float? lifetimeMax;
            public float? lifetimeMin;
            public float? initSpeedRatio;
        }

        private int _lifeFrameMax;

        protected override void Start()
        {
            base.Start();
            _lifeFrameMax = (int)(vfx.GetFloat(_lifetimeMax) * 60);
        }

        public void EffectExe(EffectPar par)
        {
            switch (par.effectMode)
            {
                case EffectMode.Constant:
                    vfx.SendEvent(_constantSpawnEvent);
                    break;
                case EffectMode.Burst:
                    vfx.SendEvent(_burstSpawnEvent);
                    break;
                default:
                    throw new System.NotImplementedException();
            }

            if (par.spawnPos != null) vfx.SetVector3(_spawnPos, par.spawnPos.Value);
            if (par.effectRotate != null) vfx.SetVector3(_effectRotate, par.effectRotate.Value);
            if (par.power != null) vfx.SetFloat(_power, par.power.Value);
            if (par.lifetimeMax != null) vfx.SetFloat(_lifetimeMax, par.lifetimeMax.Value);
            if (par.lifetimeMin != null) vfx.SetFloat(_lifetimeMin, par.lifetimeMin.Value);
            if (par.spawnNum != null) vfx.SetFloat(_spawnNum, par.spawnNum.Value);
            if (par.outputYAngle != null) vfx.SetFloat(_outputYAngle, par.outputYAngle.Value);
            if (par.outputYDirection != null) vfx.SetFloat(_outputYDirection, par.outputYDirection.Value);
            if (par.outputZAngleMax != null) vfx.SetFloat(_outputZAngleMax, par.outputZAngleMax.Value);
            if (par.outputZAngleMin != null) vfx.SetFloat(_outputZAngleMin, par.outputZAngleMin.Value);
            if (par.outputMagnitudeMax != null) vfx.SetFloat(_outputMagnitudeMax, par.outputMagnitudeMax.Value);
            if (par.outputMagnitudeMin != null) vfx.SetFloat(_outputMagnitudeMin, par.outputMagnitudeMin.Value);
            if (par.spawnPosSize != null) vfx.SetFloat(_spawnPosSize, par.spawnPosSize.Value);
            if (par.initSpeedRatio != null) vfx.SetFloat(_initSpeedRatio, par.initSpeedRatio.Value);

            vfx.Simulate(1f / 60f, 1);
            stopFrame = CalcStopFrameOnVfxStop();
        }

        protected override int CalcStopFrameOnVfxStop()
        {
            return ActionManager.Inst.actionFrame + _lifeFrameMax;
        }
    }
}