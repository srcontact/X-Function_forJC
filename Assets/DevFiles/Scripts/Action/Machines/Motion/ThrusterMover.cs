using clrev01.Bases;
using clrev01.ClAction.Effect.Thruster;
using clrev01.Extensions;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Machines.Motion
{
    public class ThrusterMover : BaseOfCL
    {
        [System.Serializable]
        public class ThrusterObj
        {
            public AimIK ik;

            [SerializeField]
            private ThrusterVfxControl origThrusterVfx;
            [SerializeField, Range(0f, 1f)]
            private float rotateRatio = 0.2f;
            private float _nowPositionWeight;
            [SerializeField]
            private float thrustAngleMargin = 45;
            [SerializeField]
            private float thrustPowRatio = 0.5f;

            private Transform _parentTransform;

            public Transform parentTransform
            {
                get
                {
                    if (_parentTransform == null) _parentTransform = ik.transform.parent;
                    return _parentTransform;
                }
            }

            [ReadOnly]
            public ThrusterVfxControl thrusterVfx;

            private List<(RotationLimit rotationLimit, Transform transform)> _rotationLimits;
            [SerializeField]
            private bool stopOnGrounded;

            public void InitializeVfxControl()
            {
                var ikTransform = ik.solver.bones[^1].transform;
                parentTransform.InverseTransformDirection(ikTransform.up);
                if (origThrusterVfx != null)
                {
                    thrusterVfx = origThrusterVfx.SafeInstantiate();
                    thrusterVfx.gameObject.transform.parent = ikTransform;
                    thrusterVfx.lpos = Vector3.zero;
                    thrusterVfx.lrot = Quaternion.identity;
                }
            }

            public bool IsInThrustAngleMargin(Vector3 thrustVector)
            {
                if (thrustVector.sqrMagnitude == 0) return false;
                var thrustVectorQ = Quaternion.LookRotation(thrustVector) * Quaternion.Euler(-90, 0, 0);
                var initThrustVectorQ = thrustVectorQ;
                var bones = ik.solver.bones;
                for (int i = bones.Length - 2; i >= 0; i--)
                {
                    var bone = bones[i];
                    if (bone.rotationLimit is null) return true;
                    thrustVectorQ = bone.rotationLimit.GetLimitedLocalRotation(thrustVectorQ, out var isNotComplete);
                    if (!isNotComplete) return true;
                }

                return Quaternion.Angle(initThrustVectorQ, thrustVectorQ) <= thrustAngleMargin;
            }

            public void ThrusterMove(bool thrusterOn, Vector3 thrusterVector, float thrusterPower, bool grounded)
            {
                if (thrusterOn && (!grounded || !stopOnGrounded) && thrusterVector.sqrMagnitude != 0)
                {
                    var tgtPos = thrusterVector.normalized * 10000;
                    ik.solver.SetIKPosition(tgtPos);
                    ik.solver.IKPositionWeight = _nowPositionWeight = Mathf.Lerp(_nowPositionWeight, 1, rotateRatio);
                }
                else
                {
                    ik.solver.IKPositionWeight = _nowPositionWeight = stopOnGrounded && grounded ? 0 : Mathf.Lerp(_nowPositionWeight, 0, rotateRatio);
                }

                if (thrusterVfx != null)
                {
                    if (thrusterOn && Vector3.Angle(thrusterVector, thrusterVfx.transform.up) <= thrustAngleMargin)
                    {
                        thrusterVfx.ThrusterExe(thrusterPower * thrustPowRatio);
                    }
                    else
                    {
                        thrusterVfx.VfxStopImmediately();
                    }
                }
            }
        }

        [SerializeField]
        List<ThrusterObj> thrusterObjs = new();

        [SerializeField]
        GroundEffectVfxControl origGroundEffectVfx;

        [ReadOnly]
        public GroundEffectVfxControl groundEffectVfx;

        public bool legMoveDisableOnThrust;


        private void OnEnable()
        {
            foreach (var item in thrusterObjs)
            {
                item.InitializeVfxControl();
            }

            groundEffectVfx = origGroundEffectVfx.SafeInstantiate();
            groundEffectVfx.gameObject.transform.parent = transform;
            groundEffectVfx.lpos = Vector3.zero;
            groundEffectVfx.lrot = quaternion.identity;
        }

        /// <summary>
        /// スラスト実行
        /// </summary>
        /// <param name="thrusterOn">スラスタ作動フラグ</param>
        /// <param name="grounded"></param>
        /// <param name="thrusterPower"></param>
        /// <param name="groundEffectPower"></param>
        /// <param name="thrusterVector">スラスタ出力ベクトル（グローバル空間）</param>
        /// <param name="groundEffectPos"></param>
        /// <param name="groundEffectNormal"></param>
        public void ThrusterExe(
            bool thrusterOn,
            bool grounded = false,
            float thrusterPower = 0,
            float groundEffectPower = 0,
            Vector3 thrusterVector = new(),
            Vector3? groundEffectPos = null,
            Vector3? groundEffectNormal = null)
        {
            foreach (var item in thrusterObjs)
            {
                item.ThrusterMove(thrusterOn, thrusterVector, thrusterPower, grounded);
            }

            if (thrusterOn && groundEffectPos.HasValue && groundEffectNormal.HasValue)
            {
                float effectOutputYAngle =
                    Mathf.Clamp(360 - Vector3.Angle(groundEffectNormal.Value, -thrusterVector) * 4, 0, 360);
                groundEffectVfx.EffectExe(new GroundEffectVfxControl.EffectPar()
                {
                    spawnPos = groundEffectPos.Value,
                    effectRotate = VectorFollowTheGround(thrusterVector, groundEffectNormal.Value).eulerAngles,
                    power = groundEffectPower,
                    outputYAngle = effectOutputYAngle
                });
            }
            else
            {
                groundEffectVfx.VfxStop();
            }
        }
    }
}