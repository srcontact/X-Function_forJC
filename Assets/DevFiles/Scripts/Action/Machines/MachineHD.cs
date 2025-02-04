using clrev01.Bases;
using clrev01.ClAction.Machines.Motion;
using clrev01.ClAction.Machines.RunningActionHolder;
using clrev01.ClAction.Radar;
using clrev01.ClAction.Shield;
using clrev01.Save;
using System;
using System.Collections.Generic;
using clrev01.ClAction.Effect;
using clrev01.ClAction.Effect.Smoke;
using clrev01.Programs.FuncPar;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using static clrev01.Bases.UtlOfCL;
using static clrev01.Extensions.ExPrediction;

namespace clrev01.ClAction.Machines
{
    [RequireComponent(typeof(Rigidbody))]
    public class MachineHD : Hard<MachineCD, MachineLD, MachineHD>, IShieldUser, IGroundableHD, IIkUser
    {
        public Collider moveCollider;
        public Transform bodyTransform;
        [SerializeField]
        private List<IK> _ikList = new();
        public List<IK> ikList => _ikList;
        [SerializeField]
        private MachineAnimationController animationController;
        public bool guardActionAvailable => animationController.guardActionAvailable;
        public bool coverActionAvailable => animationController.coverActionAvailable;
        public LegMover legMover;
        public ThrusterMover thrusterMover;
        [NonSerialized]
        public ShieldHD shieldHd;
        public ShieldCD shieldCd => ld.shieldCd;

        public List<BaseAimIkPar> aimList = new();
        [SerializeField]
        private List<Transform> shootPoints = new();
        public List<Transform> useShootPoints = new();
        public FightMover fightMover;
        private RadarSymbol radarSymbol;
        [NonSerialized]
        public int currentFightMotion = -1;

        private List<Collider> ignoreFightHitColliders;
        public List<ContactPoint?> groundContacts => ld.movePar.groundContacts;
        public bool touchGround
        {
            get => ld.movePar.touchGround;
            set => ld.movePar.touchGround = value;
        }
        [SerializeField]
        private VfxSlaveObjectCD damageSmokeSlaveObjectCd;
        private VfxSlaveObjectHD _damageSmokeSlaveObject;
        [ShowInInspector, ReadOnly]
        private DamageSmokeVfxControl _damageSmokeVfxControl;
        [SerializeField]
        private VfxSlaveObjectCD explosionSlaveObjCd;
        private VfxSlaveObjectHD _explosionSlaveObj;

        public override void Awake()
        {
            base.Awake();
            radarSymbol = StaticInfo.Inst.radarSymbolHub.GetMachineSymbol();
            radarSymbol.transform.SetParent(transform);
            radarSymbol.lpos = Vector3.zero;
            radarSymbol.lrot = Quaternion.identity;
            fightMover.ExeOnMachineHdInit();
            ignoreFightHitColliders = new List<Collider>();
            ignoreFightHitColliders.AddRange(colliderList);
            objectSearchTgt.aimingAtMeDict = new();
            if (damageSmokeSlaveObjectCd != null) damageSmokeSlaveObjectCd.StandbyPoolActors(1);
        }

        private void OnEnable()
        {
            if (damageSmokeSlaveObjectCd != null)
            {
                var position = _damageSmokeVfxControl != null ? _damageSmokeVfxControl.pos : pos;
                _damageSmokeSlaveObject = damageSmokeSlaveObjectCd.InstActor(position, rot);
                _damageSmokeSlaveObject.Init(gameObject);
                _damageSmokeVfxControl = (DamageSmokeVfxControl)_damageSmokeSlaveObject.vfxController;
            }
        }

        public void SetAdditionalTurrets()
        {
            useShootPoints.Clear();
            useShootPoints.AddRange(shootPoints);

            foreach (var aimIkPar in aimList)
            {
                if (aimIkPar is not AdditionalTurretAimIkPar atAimIkPar) continue;
                if (atAimIkPar.additionalTurretObj)
                {
                    atAimIkPar.additionalTurretObj.gameObject.SetActive(false);
                    atAimIkPar.additionalTurretObj = null;
                }
                var weaponCode = ld.customData.mechCustom.weapons[aimIkPar.correspondingWeaponNum];
                var additionalTurretObj = WHUB.GetAdditionalTurretObj(weaponCode);
                if (!additionalTurretObj) continue;
                additionalTurretObj.transform.SetParent(atAimIkPar.additionalTurretSetTransform, false);
                additionalTurretObj.scl = atAimIkPar.additionalTurretScale;
                additionalTurretObj.lrot = Quaternion.Euler(atAimIkPar.additionalTurretRotation);
                atAimIkPar.additionalTurretObj = additionalTurretObj;
                var additionAimIK = additionalTurretObj.aimIK;
                List<Transform> boneList = new List<Transform>();
                foreach (var bone in atAimIkPar.additionalTurretIkTransforms)
                {
                    boneList.Add(bone);
                }
                foreach (var bone in additionAimIK.solver.bones)
                {
                    boneList.Add(bone.transform);
                }
                additionAimIK.solver.SetChain(boneList.ToArray(), additionAimIK.solver.GetRoot());
                ikList.Add(additionAimIK);

                useShootPoints[aimIkPar.correspondingWeaponNum] = atAimIkPar.additionalTurretObj.shootPoint;

                atAimIkPar.SetDefaultLocalPos();
            }
        }

        public void Start()
        {
            if (shieldCd != null)
            {
                shieldHd = shieldCd.InstActor(pos, rot);
                shieldHd.Init(gameObject);
                shieldHd.InitializeShield(this, colliderList, teamID, transform);
                ignoreFightHitColliders.AddRange(shieldHd.colliderList);
            }
            (this as IIkUser).InitIkOnAwake();
        }

#if UNITY_EDITOR
        public override void OnValidate()
        {
            base.OnValidate();
            UnityEditor.EditorApplication.delayCall += _OnValidate;
            _ikList.Clear();
            _ikList.AddRange(GetComponentsInChildren<IK>());
        }

        /// <summary>
        /// OnValidateで直接実行するとエラーが出るので遅延実行。
        /// 以下を参考。
        /// https://www.create-forever.games/unitysendmessage-cannot-be-called-during-awake-checkconsistency-or-onvalidate/
        /// </summary>
        private void _OnValidate()
        {
            UnityEditor.EditorApplication.delayCall -= _OnValidate;
            foreach (var ap in aimList)
            {
                ap.SetInfo();
            }
            fightMover.SetInfos();
        }
#endif

        public override void RunBeforePhysics()
        {
            base.RunBeforePhysics();
            rigidBody.mass = ld.customData.mechCustom.CalcWeightSum() / 1000;
            if (ld.actionState is ActionState.Uncontrollable || ld.statePar.destroyedFrame > 0) PhysicsLayerChangeOnUncontrollable();
            else PhysicsLayerChangeOnNormal();
            AddImpactEffect();
            if (shieldHd)
            {
                shieldHd.UpdateShield(rigidBody.linearVelocity);
                shieldHd.HitDetectInside();
            }
        }

        public override void RunAfterPhysics()
        {
            if (ld.runningFightHolder.RunningAction != null)
            {
                FightExe();
                AimDefaultPositionSet();
            }
            else if (ld.AimingActive)
            {
                AimTgtSet();
            }
            else
            {
                AimDefaultPositionSet();
            }

            var f = thrusterMover != null && thrusterMover.legMoveDisableOnThrust && ld.movePar.moveState is CharMoveState.inAir;
            LegMoveExe(f && ld.movePar.thrustVector.sqrMagnitude > float.Epsilon, ld.actionState);
            legMover.GroundEffectUpdate(rigidBody.linearVelocity, ld.runningMoveTypeHolder.RunningAction, ld);
            ThrusterMoveExe();
            DamageSmokeExe();

            Vector3 animDir3 = ld.runningMoveTypeHolder.RunningAction is not null ? transform.InverseTransformVector(ld.runningMoveTypeHolder.RunningAction.MoveDirection(ld)) : Vector3.zero;
            if (!fightMover.nowMotionData || !fightMover.nowMotionData.overrideAnim)
            {
                animationController?.UpdateAnimation((int)ld.actionState, animDir3);
            }

            base.RunAfterPhysics();
            if (shieldHd != null) shieldHd.UpdateColor();
        }

        public override void RunOnAfterFixedUpdateAndAnimation()
        {
            base.RunOnAfterFixedUpdateAndAnimation();
            ld.ShootExe();
        }

        public override void OnDotonExe()
        {
            OnDotonResetPosition();
        }

        private void AimTgtSet()
        {
            foreach (var aimPar in aimList)
            {
                if (!aimPar.useAimIk) continue;

                RunningShootHolder rsh;
                float maxRotateAnglePerFrame;
                Vector3 wobble;
                if (aimPar.correspondingWeaponNum > -1 && ld.customData.mechCustom.weapons[aimPar.correspondingWeaponNum] != 0)
                {
                    rsh = ld.runningShootHolder[aimPar.correspondingWeaponNum];
                    var aimSpeedRatio = rsh.bullet?.AimSpeedRatio ?? 1;
                    maxRotateAnglePerFrame = ld.cd.usableWeapons[aimPar.correspondingWeaponNum].maxRotateAnglePerFrame * aimSpeedRatio;
                    wobble = rsh.nowWeaponWobble;
                    if (wobble.sqrMagnitude > 0)
                    {
                        maxRotateAnglePerFrame *= rsh.bullet?.AimSpeedRatioOnFiring ?? 1;
                    }
                }
                else
                {
                    rsh = ld.runningShootHolder[ld.GetLatestTgtChangeWeapon];
                    maxRotateAnglePerFrame = ld.cd.defaultWeaponMaxRotateAnglePerFrame;
                    wobble = Vector3.zero;
                }
                var runningAction = rsh.RunningAction;
                float? posAccuracy = null;
                if (runningAction == null)
                {
                    if (rsh.AimTgt != null)
                    {
                        posAccuracy = rsh.AimTgt.GetPosAccuracy(pos, ld.searchParameterData, objectSearchTgt.jammedSize);
                        var aimTgtPos = rsh.AimTgt.GetTargetPosition(teamID, uniqueID, ld.searchParameterData, posAccuracy.Value);
                        aimPar.Targeting(aimTgtPos, transform, bodyTransform, maxRotateAnglePerFrame, wobble, posAccuracy);
                    }
                    else aimPar.DefaultPositioning(transform, bodyTransform, rigidBody.linearVelocity);
                    continue;
                }
                Vector3 targetingPos;
                switch (runningAction.targetingType, rsh.AimTgt)
                {
                    case (TargetingType.None, _):
                    case (TargetingType.AimLockOnTarget, null):
                        aimPar.DefaultPositioning(transform, bodyTransform, rigidBody.linearVelocity);
                        continue;
                    case (TargetingType.AimLockOnTarget, _):
                        //発射する弾がない場合はとりあえず初速は固定値にしている
                        var bulletPureSpeed = rsh.bullet?.InitialSpeed ?? 10000;
                        posAccuracy = rsh.AimTgt.GetPosAccuracy(pos, ld.searchParameterData, objectSearchTgt.jammedSize);
                        var aimTgtPos = rsh.AimTgt.GetTargetPosition(teamID, uniqueID, ld.searchParameterData, posAccuracy.Value);
                        var aimTgtVelocity = rsh.AimTgt.GetTargetVelocity(teamID, uniqueID, ld.searchParameterData, posAccuracy.Value);
                        targetingPos = LinePrediction(
                            aimTgtPos,
                            aimTgtVelocity,
                            bulletPureSpeed,
                            rsh.bullet.DragCoefficient * ACM.actionEnvPar.globalAirBreakPar,
                            pos,
                            rigidBody.linearVelocity,
                            rsh.bullet != null && rsh.bullet.UseGravity ? ACM.actionEnvPar.globalGPowMSec : 0
                        );
                        break;
                    case (TargetingType.AimWithCoordinates, _):
                        targetingPos = runningAction.aimPositionV.GetUseValue(ld);
                        break;
                    case (TargetingType.AimWithAngle, _):
                        var hv = Mathf.Deg2Rad * runningAction.aimHorizontalAngleV.GetUseValueFloat(ld);
                        var vv = Mathf.Deg2Rad * runningAction.aimVerticalAngleV.GetUseValueFloat(ld);
                        var v = new Vector3(
                            Mathf.Sin(hv) * Mathf.Cos(vv),
                            Mathf.Sin(vv),
                            Mathf.Cos(hv) * Mathf.Cos(vv)).normalized * 10000;
                        targetingPos = pos + transform.TransformVector(v);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                aimPar.Targeting(targetingPos, transform, bodyTransform, maxRotateAnglePerFrame, wobble, posAccuracy);
            }
        }

        private void AimDefaultPositionSet()
        {
            foreach (var aimIkPar in aimList)
            {
                aimIkPar.DefaultPositioning(transform, bodyTransform, rigidBody.linearVelocity);
            }
        }

        private void FightExe()
        {
            if (!fightMover.checkEnd) fightMover.AdvanceFight(ignoreFightHitColliders, uniqueID, rigidBody.linearVelocity, transform);
        }

        private Vector3 _currentMoveVelocity;
        private void LegMoveExe(bool legMoveDisableOnThrust, ActionState actionState)
        {
            if (legMover == null || fightMover.nowMotionSegment is { legMoveOverride: true }) return;

            legMover.actionState = actionState;
            var legMoveVelocity = ld.movePar.rBody.linearVelocity;
            var accelerationGlobal = (legMoveVelocity - _currentMoveVelocity) * 60;
            _currentMoveVelocity = legMoveVelocity;
            var moveDirection = ld.runningMoveTypeHolder.RunningAction?.MoveDirection(ld) ?? transform.up;
            if (ld.movePar.moveState is not (CharMoveState.uncontrollable or CharMoveState.recovery))
            {
                legMover.BodyTilt(accelerationGlobal, ld.runningMoveTypeHolder.RunningAction?.GetPowerValue(ld) ?? 0, moveDirection);
            }

            if (!legMoveDisableOnThrust)
            {
                legMover.LegMove(legMoveVelocity, moveDirection, ld.movePar.groundNormal, ld.DuringLandingRigidity, ld.movePar.landingRigidityStartFrame, ld.movePar.landingRigidityEndFrame);
            }
        }

        private void ThrusterMoveExe()
        {
            if (thrusterMover == null) return;
            var thrustPowMagnitude = ld.movePar.thrustVector.magnitude;
            if (ld.statePar.destroyedFrame < 0 && thrustPowMagnitude > float.Epsilon && ld.statePar.energyUsed < ld.powerPlantData.energyCapacity)
            {
                thrusterMover.ThrusterExe(
                    true, ld.movePar.moveState is CharMoveState.isGrounded,
                    thrusterPower: thrustPowMagnitude / ld.cd.MoveCommonPar.thrustPow * ld.cd.MoveCommonPar.thrustEffectSizeRatio,
                    groundEffectPower: ld.movePar.thrustDustEffectSize,
                    thrusterVector: -ld.movePar.thrustVector,
                    groundEffectPos: ld.movePar.thrustGroundEffectPos,
                    groundEffectNormal: ld.movePar.thrustGroundEffectNormal
                );
            }
            else
            {
                thrusterMover.ThrusterExe(false);
            }
        }

        private void DamageSmokeExe()
        {
            if (_damageSmokeVfxControl != null)
            {
                var hpRatio = ld.statePar.damage / (float)ld.cd.maxHearthPoint;
                _damageSmokeVfxControl.EffectExe(hpRatio, pos, rigidBody.linearVelocity);
            }
        }

        public void ExplosionExe()
        {
            if (explosionSlaveObjCd != null)
            {
                var position = _explosionSlaveObj != null ? _explosionSlaveObj.pos : pos;
                _explosionSlaveObj = explosionSlaveObjCd.InstActor(position, rot);
                _explosionSlaveObj.Init(gameObject);
                (_explosionSlaveObj.vfxController as FixedStopVfxController)?.EffectExe();
            }
        }

        public override void AddDamage(int penetrationDamage, int impactDamage, int heatDamage, Vector3 impactPoint, Vector3 impactVelocity)
        {
            base.AddDamage(penetrationDamage, impactDamage, heatDamage, impactPoint, impactVelocity);
            if (ld == null) return;
            int armorThickness = ld.customData.mechCustom.armorThickness;
            penetrationDamage = ld.armorTypeData.CalcPenetrationDamage(penetrationDamage, armorThickness);
            impactDamage = ld.armorTypeData.CalcImpactDamage(impactDamage, armorThickness);
            var ldRunningDefenceHolder = ld.runningDefenceHolder;
            if (ldRunningDefenceHolder.RunningAction != null)
            {
                var machineMoveCommonPar = ld.cd.MoveCommonPar;
                impactDamage = (int)(impactDamage * ldRunningDefenceHolder.RunningAction.actionNum switch
                {
                    DefenceActionType.Guard =>
                        ACM.actionFrame - ldRunningDefenceHolder.actionStartFrame <= machineMoveCommonPar.justGuardFrame ? machineMoveCommonPar.justGuardReductionRate : machineMoveCommonPar.guardReductionRate,
                    DefenceActionType.Cover => machineMoveCommonPar.coverReductionRate,
                    _ => throw new ArgumentOutOfRangeException()
                });
            }
            heatDamage = ld.armorTypeData.CalcHeatDamage(heatDamage, armorThickness);
            ld.statePar.damage += penetrationDamage + impactDamage + heatDamage;
            ld.statePar.heat += heatDamage;
            ld.statePar.currentFrameImpactValue += impactDamage;
            var cdBaseImpactPhysicsRate = impactVelocity.normalized * impactDamage / ld.cd.baseImpactPhysicsRate;
            rigidBody.AddForceAtPosition(cdBaseImpactPhysicsRate, impactPoint, ForceMode.VelocityChange);
        }

        public void AddImpactEffect()
        {
            var currentFrameImpactValue = ld.statePar.currentFrameImpactValue;
            ld.statePar.currentFrameImpactValue = 0;
            var fixedImpactDamage = ld.movePar.moveState is CharMoveState.isGrounded ? (int)Mathf.Max(currentFrameImpactValue - ld.cd.baseImpactResistValue, 0) : currentFrameImpactValue;
            ld.statePar.impact += fixedImpactDamage;
        }

        public void AddShieldDamage(int damage, Vector3 impactV, float shieldSize)
        {
            if (ld.shieldCd == null || ld.statePar.latestShieldStartFrame == ACM.actionFrame) return;
            //ld.statePar.shieldDamage += damage - (int)Mathf.Min(damage, damage * ld.ShieldPar.damageReduceRate / shieldSize);
            ld.statePar.shieldDamage += (int)(damage * ld.shieldCd.damageReduceRate * shieldSize);
            rigidBody.AddForce(impactV / ld.cd.baseImpactPhysicsRate, ForceMode.VelocityChange);
        }


        private void PhysicsLayerChangeOnUncontrollable()
        {
            foreach (var col in colliderList)
            {
                if (1 << col.gameObject.layer == layerOfMachine)
                {
                    col.includeLayers |= layerOfGround;
                    col.material = ld.movePar.moveCommonPar.uncontrolablePM;
                }
                if (1 << col.gameObject.layer == layerOfTouchCollision)
                {
                    col.excludeLayers |= layerOfGround;
                }
            }
        }

        private void PhysicsLayerChangeOnNormal()
        {
            foreach (var col in colliderList)
            {
                if (1 << col.gameObject.layer == layerOfMachine)
                {
                    col.includeLayers &= ~layerOfGround;
                    col.material = ld.movePar.moveCommonPar.neutralPM;
                }
                if (1 << col.gameObject.layer == layerOfTouchCollision)
                {
                    col.excludeLayers &= ~layerOfGround;
                }
            }
        }

        public void SetRadarSymbolMat(int teamNum)
        {
            radarSymbol.SetMaterial(StaticInfo.Inst.radarSymbolHub.GetMachineSymbolMat(teamNum));
        }
    }
}