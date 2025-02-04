using clrev01.Bases;
using clrev01.ClAction.Machines.Motion;
using clrev01.Extensions;
using clrev01.HUB;
using clrev01.Programs.FuncPar;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Machines
{
    [System.Serializable]
    public class MachineMovePar : IMovePar<MachineMoveCommonPar>
    {
        public MachineMoveCommonPar moveCommonPar { get; set; }
        public Collider moveCollider { get; }
        public Vector3 accele;
        public Vector3 jumpV;
        public Vector3 impactV;
        public Rigidbody rBody;
        private Transform _rBodyTransform;
        public Transform rBodyTransform
        {
            get
            {
                if (_rBodyTransform == null) _rBodyTransform = rBody.transform;
                return _rBodyTransform;
            }
        }
        /// <summary>
        /// この速度に向けて減速する。
        /// 歩行など、一定速度で動く動きに用いる。
        /// </summary>
        public Vector3 tgtSpeed;
        public Vector3 rotateTgtSpeed;
        public Vector3 thrustVector;
        public bool nowBreakSleeping;
        [ShowInInspector, ReadOnly]
        private CharMoveState _moveState;
        public CharMoveState moveState
        {
            get => _moveState;
            set => _moveState = value;
        }
        public int uncontCount = 0;
        public RaycastHit rayHit;
        public bool touchGround;
        public bool isGrounded;
        public int inAirFixCount;
        public Vector3 currentVelocity;
        public int landingRigidityStartFrame = -1;
        public int landingRigidityEndFrame = -1;
        public List<ContactPoint?> groundContacts = new();
        public Vector3 groundNormal;
        public bool thrustUpper;
        public Vector3? thrustGroundEffectPos;
        public Vector3? thrustGroundEffectNormal;
        public float thrustDustEffectSize;


        public MachineMovePar(Collider moveCollider, int landingRigidityFrameInit)
        {
            this.moveCollider = moveCollider;
            landingRigidityStartFrame = landingRigidityEndFrame = landingRigidityFrameInit;
        }

        public void StateCheck(MachineLD ld)
        {
            thrustUpper = true;
            isGrounded = IsGroundCheck();
            if (moveState == CharMoveState.uncontrollable)
            {
                RecoveryStartCheck(ld);
            }
            else if (moveState == CharMoveState.recovery)
            {
                RecoveryEndCheck();
            }
            else if (isGrounded)
            {
                // thrustUpper = thrustVector.sqrMagnitude > Vector3.kEpsilonNormalSqrt && Vector3.Angle(thrustVector, groundNormal) < 90;
                thrustUpper = ld.runningThrustHolder.RunningAction is not null;
                FallDownSpeedCheck();
                if (FallDownSpeedCheck() || FallDownImpactCheck(ld.cd.baseStability, ld.statePar.impact)) return;
                if (moveState != CharMoveState.isGrounded)
                {
                    moveState = CharMoveState.isGrounded;
                    if (ld.runningFightHolder.RunningAction != null)
                    {
                        ld.statePar.impact += ld.hd.fightMover.nowMotionData.landingRigidityFrame * ld.cd.stabilityRecoveryRate;
                    }
                    else
                    {
                        var verticalSpeed = currentVelocity.magnitude - Vector3.ProjectOnPlane(currentVelocity, groundNormal).magnitude;
                        if (verticalSpeed > moveCommonPar.landingRigidityMinSpeed)
                        {
                            ld.statePar.impact += ld.cd.MoveCommonPar.landingRigidityImpactRate * verticalSpeed;
                        }
                    }
                    rBody.linearVelocity = Vector3.ProjectOnPlane(rBody.linearVelocity, groundNormal);
                }
                if (ld.statePar.impact > 0)
                {
                    if (landingRigidityEndFrame <= ACM.actionFrame) landingRigidityStartFrame = ACM.actionFrame;
                    var impactEndFrame = ACM.actionFrame + (int)(ld.statePar.impact / ld.cd.stabilityRecoveryRate);
                    if (landingRigidityEndFrame < impactEndFrame) landingRigidityEndFrame = impactEndFrame;
                }
            }
            else
            {
                if (FallDownImpactCheck(ld.cd.baseStability, ld.statePar.impact)) return;
                if (ld.DuringLandingRigidity) return;
                moveState = CharMoveState.inAir;
                currentVelocity = rBody.linearVelocity;
                inAirFixCount--;
            }
        }

        public void Move(MachineLD ld)
        {
            if (nowBreakSleeping) return;
            MoveCalc(ld);
            MoveExe();
        }
        void MoveCalc(MachineLD ld)
        {
            if (moveState == CharMoveState.uncontrollable) return;
            else if (moveState == CharMoveState.isGrounded)
            {
                CalcGroundMove(ld);
                CalcResistGravity();
                CalcDownforce(ld);
            }
            else
            {
                CalcAirMove();
            }
        }
        void MoveExe()
        {
            if (Mathf.Abs(thrustVector.sqrMagnitude) > float.Epsilon)
            {
                rBody.AddForce(thrustVector, ForceMode.Acceleration);
            }
            if (moveState == CharMoveState.uncontrollable) return;
            if (moveState == CharMoveState.recovery)
            {
                RecoveryMove();
                return;
            }
            if (Mathf.Abs(accele.sqrMagnitude) > float.Epsilon)
            {
                rBody.AddForce(accele, ForceMode.Acceleration);
            }
            if (Mathf.Abs(jumpV.sqrMagnitude) > float.Epsilon)
            {
                rBody.AddForce(jumpV, ForceMode.VelocityChange);
            }
            RotateExe();
        }

        void RotateExe()
        {
            var localAngularVelocity = rBodyTransform.InverseTransformVector(rBody.angularVelocity);
            var tgtRotation = Quaternion.FromToRotation(Vector3.up, rBodyTransform.InverseTransformVector(groundNormal));

            float gain = 0, xzGain = 0, xzMaxSpeed = 0;
            switch (moveState)
            {
                case CharMoveState.isGrounded:
                    gain = moveCommonPar.rotateGainGrounded;
                    xzGain = moveCommonPar.rotateXzGainGrounded;
                    xzMaxSpeed = moveCommonPar.rotateXzMaxSpeedGrounded;
                    break;
                case CharMoveState.inAir:
                    gain = moveCommonPar.rotateGainInAir;
                    xzGain = moveCommonPar.rotateXzGainInAir;
                    xzMaxSpeed = moveCommonPar.rotateXzMaxSpeedAir;
                    break;
            }

            var autoRotateXzTgtSpeed = new Vector3(tgtRotation.x, 0, tgtRotation.z) * (xzMaxSpeed * Mathf.Deg2Rad);

            localAngularVelocity.y += (rotateTgtSpeed.y - localAngularVelocity.y) * gain;
            localAngularVelocity.x += (autoRotateXzTgtSpeed.x - localAngularVelocity.x) * xzGain;
            localAngularVelocity.z += (autoRotateXzTgtSpeed.z - localAngularVelocity.z) * xzGain;
            rBody.angularVelocity = rBodyTransform.TransformVector(localAngularVelocity);
        }
        void CalcGroundMove(MachineLD ld)
        {
            Vector3 fixedV = Vector3.ProjectOnPlane(rBody.linearVelocity, groundNormal);
            float gain = 0;
            if (ld.hd.legMover.legsGrounded)
            {
                var weightPar = ld.CalcWeightPar();
                switch (ld.actionState)
                {
                    case ActionState.Neutral:
                        jumpV += -fixedV.normalized * (Mathf.Min(fixedV.magnitude, moveCommonPar.neutralAccell) * moveCommonPar.breakGainWeightCurve.Evaluate(weightPar));
                        break;
                    case ActionState.Move:
                        gain = moveCommonPar.moveGain * moveCommonPar.moveGainWeightCurve.Evaluate(weightPar);
                        break;
                    case ActionState.Dash:
                        var velocity = ld.hd.rigidBody.linearVelocity;
                        var dashWeightPar = moveCommonPar.dashGainWeightCurve.Evaluate(weightPar);
                        gain = moveCommonPar.dashGain * dashWeightPar;
                        if (velocity.sqrMagnitude > moveCommonPar.dashGainRatioMinimumEffectiveSpeed * moveCommonPar.dashGainRatioMinimumEffectiveSpeed)
                        {
                            var dot = Vector3.Dot(tgtSpeed.normalized, velocity.normalized);
                            gain *= ld.cd.MoveCommonPar.dashGainRatioAtChangingDirection.Evaluate(dot);
                        }
                        break;
                    case ActionState.Jump:
                        break;
                    case ActionState.InAir:
                        break;
                    case ActionState.Breaking:
                        break;
                    case ActionState.Cover:
                        break;
                    case ActionState.Guard:
                        break;
                    case ActionState.Uncontrollable:
                        break;
                    case ActionState.Recovery:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            jumpV += (tgtSpeed - fixedV) * gain;
        }

        void CalcResistGravity()
        {
            Vector3 gravityAcceleOnGround = Vector3.ProjectOnPlane(Physics.gravity, groundNormal);
            var gravityMagnitude = Physics.gravity.magnitude;
            if (gravityAcceleOnGround.magnitude <= gravityMagnitude * moveCommonPar.resistGravityRate)
            {
                accele -= gravityAcceleOnGround;
            }
            else
            {
                accele -= gravityAcceleOnGround.normalized * (gravityMagnitude * moveCommonPar.resistGravityRate);
            }
        }
        private void CalcDownforce(MachineLD ld)
        {
            //以下の条件の場合ダウンフォースをかけない
            //・接地状態でない
            //・スラストを上昇方向にかけている
            //・ダウンフォースが無効な移動アクションを実行中
            //・接地コライダーが接地しているかつ、移動したい方向に現在の移動ベクトルが一定以上ない

            if (!isGrounded ||
                thrustUpper ||
                ld.actionState is ActionState.Jump or ActionState.InAir or ActionState.Uncontrollable) return;

            var moveAccele = jumpV + accele / 60;
            var moveAcceleNormalized = moveAccele.normalized;
            var rbodyVelocity = rBody.linearVelocity;
            if (touchGround && Vector3.Project(rbodyVelocity, moveAcceleNormalized).magnitude * Vector3.Dot(moveAcceleNormalized, rbodyVelocity.normalized) <= 10) return;

            var moveVector = rbodyVelocity + moveAccele;
            jumpV -= groundNormal * MathF.Max(moveVector.magnitude * moveCommonPar.groundDownforceRatio, moveCommonPar.minGroundDownforce);
        }
        public void CheckAndExeSleep()
        {
            if (rBody.linearVelocity.sqrMagnitude < moveCommonPar.breakStopSpeed * moveCommonPar.breakStopSpeed)
            {
                rBody.Sleep();
                nowBreakSleeping = true;
            }
        }
        void RecoveryEndCheck()
        {
            Vector3 rot = rBody.rotation.eulerAngles;
            if (rot is { x: < 1, z: < 1 })
            {
                moveState = isGrounded ? CharMoveState.isGrounded : CharMoveState.inAir;
            }
        }
        void RecoveryMove()
        {
            rBody.angularVelocity = Vector3.zero;
            Vector3 rot = rBodyTransform.rotation.eulerAngles;
            //if (!isGrounded) accele.y -= ACM.actionEnvPar.globalGPowMSec * moveCommonPar.groundedDownForce;
            rot.x = Mathf.MoveTowardsAngle(rot.x, 0, moveCommonPar.recoveryMoveAngle / 60f);
            rot.z = Mathf.MoveTowardsAngle(rot.z, 0, moveCommonPar.recoveryMoveAngle / 60f);
            rBodyTransform.rotation = Quaternion.Euler(rot);
        }
        void RecoveryStartCheck(MachineLD ld)
        {
            if (uncontCount > moveCommonPar.uncontRecoveryFlame)
            {
                uncontCount = 0;
                moveState = CharMoveState.recovery;
                rBody.angularVelocity = Vector3.zero;
            }
            else if (ld.statePar.latestImpactDamageFrame >= ACM.actionFrame - 1)
            {
                uncontCount = 0;
            }
            else uncontCount++;
        }
        /// <summary>
        /// バウンド処理。制御不能状態に切り替え。
        /// ＊仮実装。現状ではバウンド条件はY軸速度のみ。水平速度、着地面の法線も考慮させる予定。
        /// </summary>
        bool FallDownSpeedCheck()
        {
            if ((-moveCommonPar.absorbableSpeed).isBiggerV3(rBody.linearVelocity.y) ||
                rBody.linearVelocity.magnitude.isBiggerV3(moveCommonPar.maxControllableSpeed))
            {
                ExecuteFallDown();
                return true;
            }
            return false;
        }

        bool FallDownImpactCheck(float stability, float impact)
        {
            if (stability <= impact)
            {
                ExecuteFallDown();
                return true;
            }
            return false;
        }

        public void ExecuteFallDown()
        {
            moveState = CharMoveState.uncontrollable;
        }

        bool IsGroundCheck()
        {
            if (inAirFixCount > 0) return false;
            var rbodyV = (jumpV.magnitude == 0 ? tgtSpeed : jumpV).normalized;
            groundNormal = Vector3.zero;
            if (touchGround)
            {
                foreach (var t in groundContacts)
                {
                    if (t.HasValue)
                    {
                        var contactV = (t.Value.point - t.Value.thisCollider.bounds.center).normalized;
                        var dot = Mathf.Max(Vector3.Dot(rbodyV, contactV), 0.01f);
                        groundNormal += t.Value.normal * dot;
                    }
                }
                groundNormal = groundNormal.normalized;
                return true;
            }
            if (moveState == CharMoveState.inAir)
            {
                var bounds = moveCollider.bounds;
                Ray rayVelocity1 = new Ray(bounds.center, rBody.linearVelocity);
                Ray rayVelocity2 = new Ray(bounds.center, Vector3.down);
                if (Physics.Raycast(rayVelocity1, out rayHit, moveCommonPar.touchGroundLength, layerOfGround) ||
                    (rBody.linearVelocity.y < 0 && Physics.Raycast(rayVelocity2, out rayHit, moveCommonPar.touchGroundLength, layerOfGround)))
                {
                    groundNormal = rayHit.normal;
                    return true;
                }
                else
                {
                    groundNormal = Vector3.up;
                    return false;
                }
            }
            else
            {
                bool isGroundContacted = false;
                foreach (var t in groundContacts)
                {
                    if (!t.HasValue) continue;
                    var groundContact = t.Value;
                    var bounds = groundContact.thisCollider.bounds;
                    Ray ray1 = new Ray(bounds.center, groundContact.point - bounds.center);
                    Ray ray2 = new Ray(bounds.center, Vector3.down);
                    if (groundContact.otherCollider.Raycast(ray1, out var checkRayHit, moveCommonPar.touchGroundLength) ||
                        groundContact.otherCollider.Raycast(ray2, out checkRayHit, moveCommonPar.touchGroundLength))
                    {
                        var contactV = (checkRayHit.point - rBodyTransform.position).normalized;
                        var dot = Mathf.Max(Vector3.Dot(rbodyV, contactV), 0.01f);
                        groundNormal += checkRayHit.normal * dot;
                        isGroundContacted = true;
                    }
                }
                groundNormal = isGroundContacted ? groundNormal.normalized : Vector3.up;
                return isGroundContacted;
            }
        }
        void CalcAirMove()
        {
            AirBreakCalc();
        }
        void AirBreakCalc()
        {
            Vector3 nv = Vector3.zero;
            if (rBody.linearVelocity.y != 0)
            {
                if (rBody.linearVelocity.y.isBiggerV3(0)) nv.y -= moveCommonPar.nvreak.up * rBody.linearVelocity.y;
                else nv.y -= moveCommonPar.nvreak.down * rBody.linearVelocity.y;
            }
            if (rBody.linearVelocity.z != 0)
            {
                if (rBody.linearVelocity.z.isBiggerV3(0)) nv.z -= moveCommonPar.nvreak.forward * rBody.linearVelocity.z;
                else nv.z -= moveCommonPar.nvreak.back * rBody.linearVelocity.z;
            }
            if (rBody.linearVelocity.x != 0)
            {
                if (rBody.linearVelocity.x.isBiggerV3(0)) nv.x -= moveCommonPar.nvreak.right * rBody.linearVelocity.x;
                else nv.x -= moveCommonPar.nvreak.left * rBody.linearVelocity.x;
            }

            nv *= ACM.actionEnvPar.globalAirBreakPar;
            accele += nv;
        }

        public void ActMove(Vector3 vector, float power)
        {
            vector *= power;
            var tgtSpeedNotOnPlane = Vector3.Scale(vector, moveCommonPar.movePow.DHPforV3(vector));
            var speed = tgtSpeedNotOnPlane.magnitude;
            var rot = Quaternion.LookRotation(Vector3.ProjectOnPlane(rBodyTransform.forward, groundNormal).normalized, groundNormal);
            //機体が逆さに近いが完全に逆さでない状態で地面に接触した場合、前方の判断がひっくり返るのを防ぐため反転する。
            var dot = Vector3.Dot(rBodyTransform.up, groundNormal);
            if (dot is < 0 and > -0.5f) rot = Quaternion.Inverse(rot);
            tgtSpeed = (rot * tgtSpeedNotOnPlane).normalized * speed;
            var position = rBody.position;
            Debug.DrawLine(position, position + tgtSpeed.normalized * 10, Color.magenta, 1);
        }

        public void ActDash(Vector3 vector, float power)
        {
            vector *= power;
            var tgtSpeedNotOnPlane = Vector3.Scale(vector, moveCommonPar.dashHPow.DHPforV3(vector));
            var speed = tgtSpeedNotOnPlane.magnitude;
            var rot = Quaternion.LookRotation(Vector3.ProjectOnPlane(rBodyTransform.forward, groundNormal).normalized, groundNormal);
            //機体が逆さに近いが完全に逆さでない状態で地面に接触した場合、前方の判断がひっくり返るのを防ぐため反転する。
            var dot = Vector3.Dot(rBodyTransform.up, groundNormal);
            if (dot is < 0 and > -0.5f) rot = Quaternion.Inverse(rot);
            tgtSpeed = (rot * tgtSpeedNotOnPlane).normalized * speed;
            var position = rBody.position;
            Debug.DrawLine(position, position + jumpV.normalized * 10, Color.magenta, 1);
        }

        public void ActFightVelocityFix(Vector3 tgtDirection, FightHomingData fightHomingData)
        {
            var stateIsGrounded = moveState == CharMoveState.isGrounded;
            if (stateIsGrounded) tgtDirection = Vector3.ProjectOnPlane(tgtDirection, groundNormal);
            tgtDirection = tgtDirection.normalized;
            var velocity = rBody.linearVelocity;
            var tgtVelocity = velocity.magnitude * tgtDirection;
            var velocityDiff = tgtVelocity - velocity;
            var fixMaxSpeed =
                stateIsGrounded ? fightHomingData.groundedVelocityFixMaxSpeed : fightHomingData.inAirVelocityFixMaxSpeed;
            if (velocityDiff.magnitude > fixMaxSpeed) velocityDiff = velocityDiff.normalized * fixMaxSpeed;
            jumpV += velocityDiff;
        }

        public void ActFightThrust(Vector3 tgtDirection, FightHomingData fightHomingData, ThrusterData thrusterData)
        {
            var stateIsGrounded = moveState == CharMoveState.isGrounded;
            if (stateIsGrounded)
            {
                ActThrust(ThrustMode.Quick, Vector3.ProjectOnPlane(tgtDirection, groundNormal).normalized, thrusterData);
            }
            else
            {
                ActThrust(ThrustMode.Quick, tgtDirection, thrusterData);
            }
        }

        public void ActJump(Vector3 jVector, float jPower, JumpFuncPar.JumpAngleReference angleReference, float minVerticalAngle, float weightPar)
        {
            var powVectorGlobal = rBodyTransform.TransformVector(moveCommonPar.jumpHPow.DHPforV3(jVector) + Vector3.up * moveCommonPar.jumpVPow);
            jVector = Vector3.Scale(jVector, powVectorGlobal);

            var upwards = angleReference switch
            {
                JumpFuncPar.JumpAngleReference.Global => Vector3.up,
                JumpFuncPar.JumpAngleReference.SlopeOfGround => groundNormal,
                _ => Vector3.up,
            };
            var rot = Quaternion.LookRotation(Vector3.ProjectOnPlane(rBodyTransform.forward, upwards).normalized, upwards);
            //機体が逆さに近いが完全に逆さでない状態で地面に接触した場合、前方の判断がひっくり返るのを防ぐため反転する。
            var dot = Vector3.Dot(rBodyTransform.up, upwards);
            if (dot is < 0 and > -0.5f) rot = Quaternion.Inverse(rot);
            jVector = rot * jVector;
            // Debug.DrawRay(rBodyTransform.position, upwards * 20, Color.cyan, 30);
            // Debug.DrawRay(rBodyTransform.position, jVector.normalized * 20, Color.magenta, 30);
            // Debug.DrawRay(rBodyTransform.position, rot * Vector3.forward * 20, Color.yellow, 30);

            var angleDif = Vector3.SignedAngle(groundNormal, jVector.normalized, Vector3.Cross(groundNormal, jVector.normalized));
            var verticalAngle = (90 - minVerticalAngle);
            if (angleDif > verticalAngle)
            {
                jVector = Vector3.RotateTowards(groundNormal, jVector.normalized, verticalAngle * Mathf.Deg2Rad, 0) * jVector.magnitude;
            }

            jVector *= jPower * moveCommonPar.jumpPowWeightCurve.Evaluate(weightPar);

            jumpV = jVector;

            inAirFixCount = moveCommonPar.jumpInAirFixFrame;
        }

        public void ActBreak(MachineLD ld, float power)
        {
            var velocity = rBody.linearVelocity;
            var fixedV = Vector3.ProjectOnPlane(velocity, groundNormal);
            var weightPar = moveCommonPar.breakGainWeightCurve.Evaluate(ld.CalcWeightPar());
            var breakV = -fixedV.normalized * (Mathf.Min(fixedV.magnitude, moveCommonPar.breakingAccell) * weightPar * power);
            jumpV += breakV;
            if (ld.runningRotateHolder.RunningAction is not null)
            {
                var forwardOrBack = Mathf.CeilToInt(Vector3.Dot(ld.hd.transform.forward, velocity.normalized));
                jumpV += Vector3.ProjectOnPlane(ld.hd.transform.forward, groundNormal) * (forwardOrBack * (breakV.magnitude * moveCommonPar.driftRate));
            }
            CheckAndExeSleep();
        }

        public void ActRotate(float pow, float rotSpeed)
        {
            rotateTgtSpeed.y = pow * rotSpeed;
        }
        public void ActRotateInAcceleAndLimit(float pow)
        {
            rotateTgtSpeed.y = moveCommonPar.rotateMaxSpeed * pow * Mathf.Deg2Rad;
        }

        public void ActThrust(ThrustMode thrustMode, Vector3 vector, ThrusterData thrusterData)
        {
            thrustVector = rBodyTransform.TransformVector(vector) * (thrustMode is ThrustMode.Normal ? thrusterData.maxPower : thrusterData.quickThrustPower);

            Ray groundEffectActiveCheck = new Ray(rBody.position, -rBodyTransform.TransformDirection(vector.normalized));
            var thrustGroundEffectActiveLength = moveCommonPar.thrustGroundEffectActiveLength;
            var thrustDustEffectActiveLength = moveCommonPar.thrustDustEffectActiveLength;
            var thrustDustEffectSizeRatio = moveCommonPar.thrustDustEffectSizeRatio;
            var rayLength = Mathf.Max(thrustGroundEffectActiveLength, thrustDustEffectActiveLength);
            var res = Physics.Raycast(
                groundEffectActiveCheck,
                out var raycastHit,
                rayLength,
                layerOfGround);

            if (res)
            {
                if (raycastHit.distance <= thrustGroundEffectActiveLength)
                {
                    var groundEffectRatio = (thrustGroundEffectActiveLength - raycastHit.distance) / thrustGroundEffectActiveLength;
                    thrustVector += raycastHit.normal * (thrustVector.magnitude * groundEffectRatio);
                }
                thrustDustEffectSize =
                    Mathf.Lerp(thrustDustEffectSizeRatio, 0,
                        raycastHit.distance / thrustDustEffectActiveLength);
                thrustDustEffectSize = (rayLength - raycastHit.distance) / rayLength * thrustDustEffectSizeRatio;
                thrustGroundEffectPos = raycastHit.point;
                thrustGroundEffectNormal = raycastHit.normal;
            }
            else
            {
                thrustDustEffectSize = 0;
                thrustGroundEffectPos = null;
                thrustGroundEffectNormal = null;
            }
        }
    }
}