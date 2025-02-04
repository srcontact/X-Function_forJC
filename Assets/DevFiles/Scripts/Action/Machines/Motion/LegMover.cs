using clrev01.Bases;
using clrev01.ClAction.Effect.Thruster;
using clrev01.Extensions;
using clrev01.Programs.FuncPar;
using clrev01.Programs.FuncPar.FuncParType;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Machines.Motion
{
    public class LegMover : BaseOfCL
    {
        [System.Serializable]
        public class LegPar
        {
            public IK ik;
            private GroundEffectVfxControl _groundEffectVfx;
            public Vector3 nowIkPos;
            public Vector3 defaultPos;
            public Vector3 groundEffectNormal;
            /// <summary>
            /// 脚の移動の目標地点。Local座標。
            /// </summary>
            public Vector3 tgtPos;
            /// <summary>
            /// 脚の移動のスタート地点。World座標。
            /// </summary>
            [ReadOnly]
            public Vector3 startPos;
            private float _reachLength = 0;
            public float ReachLength
            {
                get
                {
                    if (_reachLength != 0) return _reachLength;
                    var points = ik.GetIKSolver().GetPoints();
                    for (var i = 1; i < points.Length; i++)
                    {
                        _reachLength += Vector3.Distance(points[i - 1].transform.position, points[i].transform.position);
                    }
                    return _reachLength;
                }
            }
            private bool _isLanding = true;
            public bool isLanding
            {
                get => _isLanding;
                set
                {
                    if (_isLanding != value) _landingStateChangeFrame = ACM.actionFrame;
                    _isLanding = value;
                }
            }
            private int _landingStateChangeFrame = -1;
            public bool nowLanding => isLanding && _landingStateChangeFrame == ACM.actionFrame;

            public void Initialize(GroundEffectVfxControl origVfx)
            {
                var p = ik.GetIKSolver().GetPoints()[^1];
                _groundEffectVfx = origVfx.SafeInstantiate();
                _groundEffectVfx.transform.SetParent(p.transform);
                _groundEffectVfx.lpos = Vector3.zero;
                _groundEffectVfx.lrot = Quaternion.identity;
                _groundEffectVfx.scl = Vector3.one;
            }

            public void MovingEffect(Vector3 moveVector, float effectSize, float spawnNum)
            {
                if (_landingStateChangeFrame != ACM.actionFrame)
                {
                    _groundEffectVfx.VfxStop();
                    return;
                }
                _groundEffectVfx.EffectExe(new GroundEffectVfxControl.EffectPar()
                {
                    spawnPos = _groundEffectVfx.pos,
                    effectRotate = moveVector.sqrMagnitude > Vector3.kEpsilon * Vector3.kEpsilon ? Quaternion.LookRotation(-moveVector, Vector3.up).eulerAngles : Vector3.zero,
                    power = effectSize,
                    spawnNum = spawnNum * effectSize,
                    outputYAngle = 180
                });
            }

            public void LandingEffect(Vector3 moveVector, float effectSize, float spawnNum)
            {
                var effectOutputYAngle =
                    Mathf.Clamp(360 - Vector3.Angle(groundEffectNormal, -moveVector) * 4, 0, 360);
                _groundEffectVfx.EffectExe(new GroundEffectVfxControl.EffectPar()
                {
                    spawnPos = _groundEffectVfx.pos,
                    effectRotate = Quaternion.LookRotation(moveVector, Vector3.up).eulerAngles,
                    power = effectSize,
                    spawnNum = spawnNum * effectSize,
                    outputYAngle = effectOutputYAngle
                });
            }

            public void BreakingEffect(Vector3 moveVector, float effectSize, float spawnNum)
            {
                if (!isLanding || moveVector.sqrMagnitude < 1)
                {
                    _groundEffectVfx.VfxStop();
                    return;
                }
                _groundEffectVfx.EffectExe(new GroundEffectVfxControl.EffectPar()
                {
                    spawnPos = _groundEffectVfx.pos,
                    effectRotate = Quaternion.LookRotation(moveVector, Vector3.up).eulerAngles,
                    power = effectSize,
                    spawnNum = spawnNum * effectSize,
                    outputYAngle = 180
                });
            }

            public void JumpEffect()
            { }
        }

        [System.Serializable]
        public class LegPair
        {
            public List<LegPar> legPair = new List<LegPar>();
        }

        public Transform armatureCenter, coreBone;
        public ActionState actionState;
        public List<LegPair> legPairList = new List<LegPair>();
        public AnimationCurve hWalkLegPos, vWalkLegPos;
        public float walkLegMaxHeight = 0.25f;
        public AnimationCurve hDashLegPos, vDashLegPosForward, vDashLegPosBack;
        public float dashLegMaxHeight = 0.25f;
        public int exeCount = 0;
        [ReadOnly]
        public int walkFrame, walkCycle = 0, stepCount;
        public int defaultWalkFrame = 5, minWalkFrame = 2;
        public int defaultDashFrame = 5, minDashFrame = 3;
        public Vector3 maxStrideLength = new Vector3(2, 0, 2.5f);
        [SerializeField]
        private Vector3 dashTgtPosOffset;
        public int jumpFrame;
        public float jumpLegHang = 2, jumpHangSide = 10, landingStandbyMinHeight = 1, landingStandbyMaxHeight = 4;
        public Vector3 hangBack, hangForward;

        public float groundingCheckStartHeight = 3;
        public float groundingCheckMaxLength = 500;
        public LayerMask touchLayer;
        public bool legsGrounded;
        public bool resetStartPosFlag, advanveWalkCycleOnExeFlag;

        public float crouchingLength = -0.00845f;
        public AnimationCurve crouchingCurve = new(new Keyframe(0, 0, 0, 2), new Keyframe(1, 1, 0, 0));
        public Vector3 LatestMoveDirection { get; private set; } = Vector3.up;
        private bool _walkCycleReverse = false;
        private float _moveDirectionAndVelocityDot;
        [Range(0, 1)]
        public float moveGroundEffectSize = 0.1f;
        public int moveGroundEffectNum = 500;
        [Range(0, 1)]
        public float dashGroundEffectSize = 0.3f;
        public int dashGroundEffectNum = 500;
        [Range(0, 1)]
        public float jumpGroundEffectSize = 0.5f;
        public int landingGroundEffectNum = 500;
        [Range(0, 1)]
        public float breakGroundEffectSize = 0.3f;
        public int breakGroundEffectNum = 500;
        public float breakGroundEffectMaxSpeed = 100;
        [SerializeField]
        private GroundEffectVfxControl origGroundEffectVfx;
        [SerializeField, BoxGroup("Body Tilt")]
        private Transform bodyTransform;
        [SerializeField, BoxGroup("Body Tilt")]
        private float moveBodyTilt = 0, dashBodyTilt = 0, breakBodyTilt = 0;
        [SerializeField, BoxGroup("Body Tilt")]
        private float referenceAccelaration = 300;
        [SerializeField, BoxGroup("Body Tilt")]
        private float bodyHorizontalizationMaxAngle = 45;
        [SerializeField, BoxGroup("Body Tilt"), Range(0, 1)]
        private float bodyHorizontalizationRatio = 0.2f;
        private Quaternion _bodyTilt;
        private Quaternion _bodyHorizontalization;


        private void Awake()
        {
            foreach (var legPair in legPairList)
            {
                foreach (var legPar in legPair.legPair)
                {
                    legPar.Initialize(origGroundEffectVfx);
                }
            }
        }

        public void BodyTilt(Vector3 accelerationGlobal, float power, Vector3 direction)
        {
            float tilt = 0;
            Vector3 crossAxis = Vector3.up;
            switch (actionState)
            {
                case ActionState.Neutral:
                case ActionState.Breaking:
                    tilt = breakBodyTilt * Mathf.Min(accelerationGlobal.magnitude / referenceAccelaration, 1);
                    crossAxis = accelerationGlobal.normalized;
                    break;
                case ActionState.Move:
                    tilt = moveBodyTilt * power;
                    crossAxis = direction.normalized;
                    break;
                case ActionState.Dash:
                    tilt = dashBodyTilt * power;
                    crossAxis = direction.normalized;
                    break;
            }
            var horizontalizeAngle = Mathf.Min(Vector3.Angle(Vector3.up, transform.up), bodyHorizontalizationMaxAngle);
            _bodyHorizontalization = Quaternion.AngleAxis(horizontalizeAngle, Vector3.Cross(transform.up, Vector3.up)) * transform.rotation;

            Quaternion tiltRotate = Quaternion.AngleAxis(-tilt, (Vector3.Cross(transform.InverseTransformVector(crossAxis), transform.up)));
            _bodyTilt = Quaternion.Lerp(_bodyTilt, tiltRotate, bodyHorizontalizationRatio);

            bodyTransform.rotation = _bodyHorizontalization;
            bodyTransform.localRotation *= _bodyTilt;
        }

        public void LegMove(Vector3 moveSpeed, Vector3 moveDirection, Vector3 groundNormal, bool duringLandingRigidity, int crouchingStartFrame, int crouchingEndFrame)
        {
#if UNITY_EDITOR
            onExe++;
#endif
            //_lpr.direction = -groundNormal;
            var coreBornePos = coreBone.localPosition;
            if (Vector3.Dot(moveDirection.normalized, LatestMoveDirection.normalized) < 0)
            {
                _walkCycleReverse = !_walkCycleReverse;
            }
            _moveDirectionAndVelocityDot = Vector3.Dot(moveSpeed.normalized, moveDirection.normalized);
            LatestMoveDirection = moveDirection;
            if (duringLandingRigidity)
            {
                var crouchingSumFrame = (float)(crouchingEndFrame - crouchingStartFrame);
                var crouchingExeFrame = (float)(crouchingEndFrame - ACM.actionFrame);
                coreBornePos.z = crouchingCurve.Evaluate(Mathf.PingPong(2 * (crouchingExeFrame / crouchingSumFrame), 1)) * crouchingLength;
            }
            else
            {
                coreBornePos.z = 0;
            }
            coreBone.localPosition = coreBornePos;
            if (advanveWalkCycleOnExeFlag)
            {
                AdvanceWalkCycle();
                advanveWalkCycleOnExeFlag = false;
            }
            switch (actionState)
            {
                case ActionState.Neutral:
                case ActionState.Breaking:
                    NeutralLegMove(moveSpeed);
                    break;
                case ActionState.Move:
                    WalkLegMove(moveSpeed);
                    break;
                case ActionState.Dash:
                    if (_moveDirectionAndVelocityDot > 0) DashLegMove(moveDirection, moveSpeed);
                    else WalkLegMove(moveSpeed);
                    break;
                case ActionState.Jump:
                case ActionState.InAir:
                    JumpLegMove(moveSpeed);
                    break;
                default:
                    return;
            }
            foreach (var legPair in legPairList)
            {
                foreach (var lp in legPair.legPair)
                {
                    var iKSolver = lp.ik.GetIKSolver();
                    iKSolver.IKPositionWeight = 1;
                }
            }
        }

        public void GroundEffectUpdate(Vector3 moveSpeed, MoveTypeFuncPar funcPar, MachineLD ld)
        {
            float pow = funcPar?.GetPowerValue(ld) ?? 1;
            switch (funcPar)
            {
                case null:
                case BreakFuncPar:
                    pow *= breakGroundEffectSize * (moveSpeed.magnitude / breakGroundEffectMaxSpeed);
                    foreach (var legPair in legPairList)
                    {
                        foreach (var lp in legPair.legPair)
                        {
                            lp.BreakingEffect(moveSpeed, pow, breakGroundEffectNum);
                        }
                    }
                    break;
                case MoveFuncPar x:
                    pow *= moveGroundEffectSize;
                    foreach (var legPair in legPairList)
                    {
                        foreach (var lp in legPair.legPair)
                        {
                            lp.MovingEffect(moveSpeed, pow, moveGroundEffectNum);
                        }
                    }
                    break;
                case DashFuncPar x:
                    pow *= dashGroundEffectSize;
                    foreach (var legPair in legPairList)
                    {
                        foreach (var lp in legPair.legPair)
                        {
                            lp.MovingEffect(moveSpeed, pow, dashGroundEffectNum);
                        }
                    }
                    break;
                case JumpFuncPar x:
                    pow *= jumpGroundEffectSize;
                    foreach (var legPair in legPairList)
                    {
                        foreach (var lp in legPair.legPair)
                        {
                            lp.LandingEffect(moveSpeed, pow, landingGroundEffectNum);
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void NeutralLegMove(Vector3 speed)
        {
            legsGrounded = true;
            foreach (var legPair in legPairList)
            {
                foreach (var lp in legPair.legPair)
                {
                    SettingTgtPos(lp, defaultWalkFrame, 0, speed, out var groundNormal);
                    NeutralSingleLegMove(lp);
                    lp.groundEffectNormal = groundNormal;
                }
            }
            SettingWaldStrideFrame(defaultWalkFrame, 0, speed);
        }

        private void NeutralSingleLegMove(LegPar lp)
        {
            lp.nowIkPos = bodyTransform.TransformPoint(lp.tgtPos);
            lp.ik.GetIKSolver().SetIKPosition(lp.nowIkPos);
            lp.isLanding = true;
        }

        private void WalkLegMove(Vector3 speed)
        {
            legsGrounded = true;
            if (exeCount == 0)
            {
                foreach (var lp in legPairList[walkCycle].legPair)
                {
                    SettingTgtPos(lp, defaultWalkFrame, minWalkFrame, speed, out var groundNormal);
                    lp.groundEffectNormal = groundNormal;
                }
                SetLegStartPos();
            }

            SettingWaldStrideFrame(defaultWalkFrame, minWalkFrame, speed);
            foreach (var lp in legPairList[walkCycle].legPair)
            {
                WalkSingleLegMove(lp, hWalkLegPos, vWalkLegPos, walkLegMaxHeight);
            }

            for (var i = 0; i < legPairList.Count; i++)
            {
                var legPair = legPairList[i];
                foreach (var legPar in legPair.legPair)
                {
                    legPar.isLanding = i != walkCycle;
                }
            }

            exeCount++;
            if (exeCount > walkFrame)
            {
                exeCount = 0;
                AdvanceWalkCycle();
            }
        }

        private void WalkSingleLegMove(LegPar lp, AnimationCurve hLegPos, AnimationCurve vLegPos, float maxHeight = 1)
        {
            float nowHs, nowVs;
            if (walkFrame > 0)
            {
                nowHs = GetCurvePar(hLegPos, walkFrame, exeCount);
                nowVs = GetCurvePar(vLegPos, walkFrame, exeCount);
            }
            else
            {
                nowHs = 1;
                nowVs = 0;
            }
            var limitedStartPos = LimitReachLength(bodyTransform.TransformPoint(lp.startPos), lp);
            lp.nowIkPos = Vector3.Lerp(limitedStartPos, bodyTransform.TransformPoint(lp.tgtPos), nowHs);
            lp.nowIkPos.y += Mathf.Min(nowVs, maxHeight);
            lp.ik.GetIKSolver().SetIKPosition(lp.nowIkPos);
        }

        private void DashLegMove(Vector3 moveDirection, Vector3 speed)
        {
            legsGrounded = false;
            if (exeCount == 0)
            {
                foreach (var lp in legPairList[walkCycle].legPair)
                {
                    Vector3 tgtPosOffset = bodyTransform.InverseTransformVector(moveDirection).normalized * dashTgtPosOffset.z;
                    SettingDashTgtPos(lp, defaultDashFrame, minDashFrame, speed, tgtPosOffset, out var groundNormal);
                    lp.groundEffectNormal = groundNormal;
                }
                SetLegStartPos();
            }
            SettingDashStrideFrame(minDashFrame, speed);
            for (int i = 0; i < legPairList.Count; i++)
            {
                foreach (var lp in legPairList[i].legPair)
                {
                    if (i == walkCycle)
                    {
                        WalkSingleLegMove(lp, hDashLegPos, vDashLegPosForward, dashLegMaxHeight);
                    }
                    else
                    {
                        var vp = GetCurvePar(vDashLegPosBack, walkFrame, exeCount) * dashLegMaxHeight;
                        var nowIkPos = lp.nowIkPos;
                        Grounding(ref lp.nowIkPos, out var gn);
                        lp.ik.GetIKSolver().SetIKPosition(nowIkPos + Vector3.up * vp);
                        if (vp <= 0)
                        {
                            legsGrounded = lp.isLanding = true;
                            lp.groundEffectNormal = gn;
                        }
                        else lp.isLanding = false;
                    }
                }
            }

            exeCount++;
            if (exeCount > walkFrame)
            {
                exeCount = 0;
                AdvanceWalkCycle();
            }
        }

        private void SettingTgtPos(LegPar lp, int defaultFrame, int minFrame, Vector3 speed, out Vector3 groundNormal)
        {
            Vector3 nv = GetWalkNextLegPos(armatureCenter.TransformPoint(lp.defaultPos), defaultFrame, minFrame, speed);
            Grounding(ref nv, out groundNormal);
            lp.tgtPos = bodyTransform.InverseTransformPoint(nv);
        }

        private void SettingDashTgtPos(LegPar lp, int defaultFrame, int minFrame, Vector3 speed, Vector3 tgtPosOffset, out Vector3 groundNormal)
        {
            var nv = armatureCenter.TransformPoint(lp.defaultPos + tgtPosOffset);
            Grounding(ref nv, out groundNormal);
            lp.tgtPos = bodyTransform.InverseTransformPoint(nv);
        }

        private void SettingDashStrideFrame(int minFrame, Vector3 speed)
        {
            var speedLocal = transform.InverseTransformVector(speed);
            walkFrame = (int)Mathf.Max(minFrame, Vector3.Project(maxStrideLength, speedLocal.normalized).magnitude / (speedLocal.magnitude / 60));
        }

        public void SetLegStartPos()
        {
            foreach (var legPair in legPairList)
            {
                foreach (var legPar in legPair.legPair)
                {
                    if (resetStartPosFlag)
                    {
                        var p = armatureCenter.TransformPoint(legPar.defaultPos);
                        Grounding(ref p, out _);
                        legPar.startPos = bodyTransform.InverseTransformPoint(p);
                    }
                    else legPar.startPos = bodyTransform.InverseTransformPoint(legPar.ik.GetIKSolver().GetIKPosition());
                }
            }
            resetStartPosFlag = false;
        }

        public void AdvanceWalkCycle()
        {
            if (!_walkCycleReverse)
            {
                walkCycle++;
            }
            else
            {
                walkCycle--;
            }
            stepCount++;
            if (walkCycle >= legPairList.Count) walkCycle = 0;
            if (walkCycle < 0) walkCycle = legPairList.Count - 1;
        }

        private Vector3 GetWalkNextLegPos(Vector3 defaultPos, int defaultFrame, int minFrame, Vector3 speed)
        {
            SettingWaldStrideFrame(defaultFrame, minFrame, speed);
            var strideLength = speed * walkFrame / 60 / 2;
            defaultPos += strideLength;
            return defaultPos;
        }
        private void SettingWaldStrideFrame(int defaultFrame, int minFrame, Vector3 speed)
        {
            var speedLocal = transform.InverseTransformVector(speed);
            walkFrame = Mathf.Min(defaultFrame, (int)Mathf.Max(minFrame, Vector3.Project(maxStrideLength, speedLocal.normalized).magnitude / (speedLocal.magnitude / 60)));
        }

        private void JumpLegMove(Vector3 moveSpeed)
        {
            legsGrounded = false;
            if (exeCount == 0)
            {
                foreach (var legPair in legPairList)
                {
                    foreach (var legPar in legPair.legPair)
                    {
                        var p = legPar.ik.GetIKSolver().GetIKPosition();
                        Grounding(ref p, out _);
                        legPar.startPos = bodyTransform.InverseTransformPoint(p);
                        legPar.isLanding = true;
                    }
                }
            }
            else
            {
                hangBack = -moveSpeed.normalized * jumpHangSide;
                hangBack.y = -jumpLegHang * jumpHangSide;
                foreach (var legPair in legPairList)
                {
                    foreach (var legPar in legPair.legPair)
                    {
                        var jp = armatureCenter.TransformPoint(legPar.defaultPos) + hangBack;
                        var limitedStartPos = LimitReachLength(bodyTransform.TransformPoint(legPar.startPos), legPar);
                        legPar.nowIkPos = Vector3.Lerp(limitedStartPos, jp, Mathf.Min(1, (float)exeCount / jumpFrame));
                        legPar.ik.GetIKSolver().SetIKPosition(legPar.nowIkPos);
                        legPar.isLanding = false;
                    }
                }
            }
            exeCount++;
        }
        private bool Grounding(ref Vector3 legPos, out Vector3 groundNormal)
        {
            Vector3 p = transform.InverseTransformPoint(legPos);
            p.y = groundingCheckStartHeight;
            var lpr = new Ray(transform.TransformPoint(p), -transform.up);
            // Debug.DrawRay(lpr.origin, lpr.direction * tgtPointMaxHeight, Color.red, 2);
            if (Physics.Raycast(lpr, out var rh, groundingCheckMaxLength, touchLayer))
            {
                legPos = rh.point;
                groundNormal = rh.normal;
                return true;
            }
            groundNormal = new Vector3();
            return false;
        }
        public void Landing(Vector3 velocity)
        {
            foreach (var legPair in legPairList)
            {
                foreach (var legPar in legPair.legPair)
                {
                    legPar.nowIkPos = LimitReachLength(armatureCenter.TransformPoint(legPar.defaultPos), legPar);
                    Grounding(ref legPar.nowIkPos, out var groundNormal);
                    legPar.ik.GetIKSolver().SetIKPosition(legPar.nowIkPos);
                }
            }
        }

        private Vector3 LimitReachLength(Vector3 tgtPos, LegPar legPar)
        {
            var root = legPar.ik.GetIKSolver().GetRoot().position;
            var diff = tgtPos - root;
            var distance = diff.magnitude;
            if (distance > legPar.ReachLength)
            {
                tgtPos = root + diff.normalized * legPar.ReachLength;
            }
            return tgtPos;
        }

        private float GetCurvePar(AnimationCurve ac, int allFrame, int nowFrame)
        {
            if (allFrame == 0) return ac.Evaluate(0);
            float cp = ac[ac.length - 1].time / allFrame * nowFrame;
            return ac.Evaluate(cp);
        }


#if UNITY_EDITOR
        public int onExe;
        public bool customParOnThis;
        [Range(0f, 1f)]
        public float tolerance = 0.1f;
        public int maxIterations = 20;
        public bool exeOnEditor;
        private void OnValidate()
        {
            if (customParOnThis)
            {
                foreach (var lp in legPairList)
                {
                    foreach (var l in lp.legPair)
                    {
                        l.ik.runInEditMode = exeOnEditor;
                        var iKSolverCCD = (IKSolverCCD)l.ik.GetIKSolver();
                        iKSolverCCD.tolerance = tolerance;
                        iKSolverCCD.maxIterations = maxIterations;
                    }
                }
            }
            if (onExe > 0 || exeOnEditor) return;
            foreach (var lp in legPairList)
            {
                foreach (var l in lp.legPair)
                {
                    var iKSolver = (IKSolverCCD)l.ik.GetIKSolver();
                    l.defaultPos = l.nowIkPos = armatureCenter.InverseTransformPoint(iKSolver.bones[^1].transform.position);
                }
            }
            MaxStrideSampling();
        }
        public bool doMaxStrideSampling = false;
        public CCDIK leftBackLeg, rightFrontLeg;
        public void MaxStrideSampling()
        {
            if (!doMaxStrideSampling || leftBackLeg == null || rightFrontLeg == null) return;
            IKSolverCCD iKSolverCCD = (IKSolverCCD)rightFrontLeg.GetIKSolver();
            var ms = iKSolverCCD.bones[^1].transform.position;
            iKSolverCCD = (IKSolverCCD)leftBackLeg.GetIKSolver();
            ms -= iKSolverCCD.bones[^1].transform.position;
            ms.y = 0;
            maxStrideLength = ms;
        }
#endif
    }
}