using clrev01.Bases;
using clrev01.ClAction.ObjectSearch;
using clrev01.ClAction.Radar;
using clrev01.Programs;
using clrev01.Programs.FieldPar;
using System;
using System.Collections.Generic;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Bullets
{
    [RequireComponent(typeof(ProjectileDirectHitting))]
    public abstract class MineHD<C, L, H> : Hard<C, L, H>, IProjectileHard, IGroundableHD
        where C : MineCD<C, L, H>
        where L : MineLD<C, L, H>, new()
        where H : MineHD<C, L, H>
    {
        public IProjectileCommonData projectileCommonData => ld.cd;
        public ProjectileDirectHitting projectileDirectHitting;
        public List<ContactPoint?> groundContacts { get; } = new();
        public bool touchGround { get; set; }


        public override void Awake()
        {
            base.Awake();
            RadarSymbol radarSymbol = StaticInfo.Inst.radarSymbolHub.GetMineSymbol();
            radarSymbol.transform.SetParent(transform);
            radarSymbol.lpos = Vector3.zero;
            radarSymbol.lrot = Quaternion.identity;
        }

        public void OnShoot(Vector3 speed, ObjectSearchTgt tgt, ObjectSearchTgt shooter, int teamId, int shooterId, int firingId = -1)
        {
            rigidBody.linearVelocity = speed;
            ld.shooterId = shooterId;
            teamID = teamId;
            ld.target = tgt;
            base.firingId = firingId;
            projectileDirectHitting.Init(ld.spawnFrame);
        }

        public override void RunBeforePhysics()
        {
            base.RunBeforePhysics();
            if (ld.isHit)
            {
                Disable();
                return;
            }

            // ヒット開始フレーム前は機体に対してヒットしないように設定
            rigidBody.excludeLayers = ld.cd.StartHittingFrame > ld.ExeFrameCount ? layerOfMachine : 0;

            ld.moveState = ld.isGrounded ? MineMoveState.Grounded : MineMoveState.InAir;
            ld.isGrounded = false;
            ExeMove();
        }

        protected abstract void ExeMove();

        public override void RunAfterPhysics()
        {
            base.RunAfterPhysics();
        }

        public override void RunOnAfterFixedUpdateAndAnimation()
        {
            base.RunOnAfterFixedUpdateAndAnimation();
            ld.isGrounded = touchGround;
            projectileDirectHitting.HitExe(ld, rigidBody, colliderList, ld.shooterId, firingId);
        }

        public override void AddDamage(int penetrationDamage, int impactDamage, int heatDamage, Vector3 impactPoint, Vector3 impactVelocity)
        {
            base.AddDamage(penetrationDamage, impactDamage, heatDamage, impactPoint, impactVelocity);
            ld.Damage += penetrationDamage + impactDamage + heatDamage;
            if (ld.Damage >= ld.cd.HealthPoint)
            {
                ld.OnHit(null, rigidBody.position, Vector3.up, HitType.DirectHit, impactVelocity);
            }
            rigidBody.AddForce(impactDamage * impactVelocity.normalized);
        }

        public override void OnDotonExe()
        {
            OnDotonResetPosition();
        }

        protected void ManageLockOn(int searchIntervalFrame, ref ObjectSearchTgt nowTarget, ObjectSearchTgt[] lockOnArray, IFieldSearchObject searchObject, float maxTrackingDistance = 0)
        {
            if (nowTarget != null && !nowTarget.gameObject.activeSelf) nowTarget = null;
            if ((ACM.actionFrame + ld.hd.uniqueID) % searchIntervalFrame == 0)
            {
                lockOnArray[0] = null;
                searchObject.LockOn(
                    UtlOfProgram.IdentificationType.Enemy,
                    UtlOfProgram.ObjType.Machine,
                    UtlOfProgram.LockOnDistancePriorityType.Near,
                    ld.hd.transform,
                    UtlOfProgram.LockOnAngleOfMovementToSelfType.None,
                    0,
                    ld.hd.teamID,
                    lockOnArray,
                    null
                );
            }

            if (lockOnArray[0] != null && (nowTarget == null || Vector3.Distance(lockOnArray[0].pos, pos) < Vector3.Distance(nowTarget.pos, pos)))
            {
                nowTarget = lockOnArray[0];
            }
            if (nowTarget != null && maxTrackingDistance > 0)
            {
                if (Vector3.Distance(nowTarget.pos, pos) > maxTrackingDistance) nowTarget = null;
            }
        }
    }
}