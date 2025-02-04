using clrev01.Bases;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Serialization;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Machines
{
    [System.Serializable]
    public class MachineMoveCommonPar
    {
        public float maxControllableSpeed;
        /// <summary>
        /// 着陸時に吸収しきれる速度。これを上回るとバウンドしてしまう。
        /// </summary>
        public float absorbableSpeed;
        public float touchGroundLength = 1;
        public int uncontRecoveryFlame = 300;
        public int recoveryMoveAngle = 20;
        /// <summary>
        /// 空気抵抗の加速度：nvreak＊速度
        /// </summary>
        public DirectionPar nvreak;
        public float minGroundDownforce = 15;
        public float groundDownforceRatio = 0.5f;
        /// <summary>
        /// 抗重力値。地上では重力加速度＊抗重力値の加速度で重力に対抗する。
        /// </summary>
        public float resistGravityRate = 0.75f;

        [BoxGroup("Neutral")]
        public float neutralGain = 4;
        [BoxGroup("Neutral")]
        public float neutralAccell = 8.4f;
        [BoxGroup("Neutral")]
        public AnimationCurve neutralGainWeightCurve = AnimationCurve.Linear(0, 1, 1, 0.25f);

        [BoxGroup("Break")]
        public float breakingGain = 15;
        [BoxGroup("Break")]
        public float breakingAccell = 100;
        [BoxGroup("Break")]
        public AnimationCurve breakGainWeightCurve = AnimationCurve.Linear(0, 1, 1, 0.25f);
        [BoxGroup("Break")]
        public float breakUseEnergy = 2;
        [BoxGroup("Break")]
        public float breakStopSpeed = 50;
        [BoxGroup("Break"), Range(0f, 1f)]
        public float driftRate = 0.75f;

        [BoxGroup("Move")]
        public DirectionHPar movePow = new() { forward = 100, back = 100, right = 100, left = 100 };
        [BoxGroup("Move")]
        public float moveGain = 10;
        [BoxGroup("Move")]
        public AnimationCurve moveGainWeightCurve = AnimationCurve.EaseInOut(0, 1, 1, 0.25f);
        [BoxGroup("Move")]
        public float moveUseEnergy = 2;

        [BoxGroup("Dash")]
        public DirectionHPar dashHPow = new() { forward = 10, back = 10, right = 10, left = 10 };
        [BoxGroup("Dash")]
        public float dashGain = 4;
        [BoxGroup("Dash")]
        public AnimationCurve dashGainWeightCurve = AnimationCurve.EaseInOut(0, 1, 1, 0.25f);
        [BoxGroup("Dash")]
        public AnimationCurve dashGainRatioAtChangingDirection = new(new Keyframe(-1, 0.5f), new Keyframe(1, 1));
        [BoxGroup("Dash")]
        public float dashGainRatioMinimumEffectiveSpeed = 75;
        [BoxGroup("Dash")]
        public float dashUseEnergy = 10;

        [BoxGroup("Jump")]
        public DirectionHPar jumpHPow = new() { forward = 100, back = 100, right = 100, left = 100 };
        [BoxGroup("Jump")]
        public float jumpVPow = 100;
        [BoxGroup("Jump")]
        public AnimationCurve jumpPowWeightCurve = AnimationCurve.Linear(0, 1, 1, 0.5f);
        [BoxGroup("Jump"), Range(0f, 90f)]
        public float jumpMinVerticalAngle = 30;
        [BoxGroup("Jump")]
        public int jumpInAirFixFrame = 2;
        [BoxGroup("Jump")]
        public float jumpUseEnergy = 100;

        [BoxGroup("Rotate")]
        public float rotateMaxSpeed = 120;
        [BoxGroup("Rotate")]
        public float rotateXzMaxSpeedGrounded = 2880;
        [BoxGroup("Rotate")]
        public float rotateXzMaxSpeedAir = 360;
        [BoxGroup("Rotate")]
        public float rotateGainGrounded = 0.2f;
        [BoxGroup("Rotate")]
        public float rotateGainInAir = 0.02f;
        [BoxGroup("Rotate")]
        public float rotateXzGainGrounded = 1f / 3;
        [BoxGroup("Rotate")]
        public float rotateXzGainInAir = 1f / 3;
        [BoxGroup("Rotate")]
        public float rotateUseEnergy = 1;

        [BoxGroup("Thrust")]
        public float thrustPow = 100;
        /// <summary>
        /// スラスタエフェクトの大きさの倍率
        /// </summary>
        [BoxGroup("Thrust")]
        public float thrustEffectSizeRatio = 1;
        /// <summary>
        /// スラスタ地面効果が発生する距離
        /// </summary>
        [BoxGroup("Thrust")]
        public float thrustGroundEffectActiveLength = 7.5f;
        /// <summary>
        /// スラスタ土煙が発生する距離
        /// </summary>
        [BoxGroup("Thrust")]
        public float thrustDustEffectActiveLength = 50f;
        /// <summary>
        /// スラスタ土煙エフェクトの大きさの倍率
        /// </summary>
        [BoxGroup("Thrust")]
        public float thrustDustEffectSizeRatio = 1.5f;
        [BoxGroup("Thrust")]
        public float thrustUseEnergy = 10;
        [BoxGroup("Thrust")]
        public float thrustHeat = 5;

        /// <summary>
        /// 着地時衝撃値が入る最小対地垂直速度
        /// </summary>
        [BoxGroup("Landing")]
        public float landingRigidityMinSpeed = 10;
        /// <summary>
        /// 着地時衝撃値レート（km/secあたり）
        /// </summary>
        [BoxGroup("Landing")]
        public float landingRigidityImpactRate = 1.6f;

        [BoxGroup("Defence")]
        public float guardBreakRatio = 0.5f;
        [FormerlySerializedAs("guardImpactDamageReductionRate")]
        [BoxGroup("Defence")]
        public float guardReductionRate = 0.5f;
        [BoxGroup("Defence")]
        public float justGuardReductionRate = 0.1f;
        [BoxGroup("Defence")]
        public int justGuardFrame = 3;
        [BoxGroup("Defence")]
        public float guardUseEnergy = 2;
        [BoxGroup("Defence")]
        public float coverBreakRatio = 0.75f;
        [FormerlySerializedAs("coverImpactDamageReductionRate")]
        [BoxGroup("Defence")]
        public float coverReductionRate = 0.5f;
        [BoxGroup("Defence")]
        public float coverUseEnergy = 2;

        public PhysicsMaterial neutralPM, uncontrolablePM;
    }
}