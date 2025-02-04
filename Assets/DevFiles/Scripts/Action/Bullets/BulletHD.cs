using clrev01.Bases;
using clrev01.ClAction.Effect.Thruster;
using clrev01.ClAction.ObjectSearch;
using clrev01.ClAction.Radar;
using Sirenix.OdinInspector;
using System;
using clrev01.ClAction.Effect;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Bullets
{
    public class BulletHD : Hard<BulletCD, BulletLD, BulletHD>, IProjectileHard
    {
        public IProjectileCommonData projectileCommonData => ld.cd;
        [SerializeField]
        private LineRenderer lineRenderer;
        [SerializeField]
        private ThrusterVfxControl thrusterVfxControl;
        [SerializeField]
        private VfxSlaveObjectCD thrusterTrailVfxSlaveCD;
        [SerializeField, ReadOnly]
        private VfxSlaveObjectHD thrusterTrailVfxSlaveObj;
        private ThrusterTrailVfxController _thrusterTrailVfxController;
        private ThrusterTrailVfxController thrusterTrailVfxController
        {
            get
            {
                if (_thrusterTrailVfxController == null)
                {
                    if (thrusterTrailVfxSlaveObj == null) return null;
                    _thrusterTrailVfxController = (ThrusterTrailVfxController)thrusterTrailVfxSlaveObj.vfxController;
                }
                return _thrusterTrailVfxController;
            }
        }
        [SerializeField, Range(0, 20)]
        float thrustTrailOffset = 10;
        public Rigidbody rBody;

        private (Collider hitCollider, Vector3 hitPos, Vector3 hitPositionNormal, HitType hitType)? _hitInfo;
        private readonly Vector3[] _positionList = new Vector3[2];

        public int hitIgnoreId { get; set; } = new();

        public override void Awake()
        {
            base.Awake();
            RadarSymbol radarSymbol = StaticInfo.Inst.radarSymbolHub.GetBulletSymbol();
            radarSymbol.transform.SetParent(transform);
            radarSymbol.lpos = Vector3.zero;
            radarSymbol.lrot = Quaternion.identity;
            if (thrusterTrailVfxSlaveCD != null) thrusterTrailVfxSlaveCD.StandbyPoolActors(1);
        }

        private void OnEnable()
        {
            if (thrusterTrailVfxSlaveCD != null)
            {
                var position = thrusterVfxControl != null ? thrusterVfxControl.pos : pos;
                thrusterTrailVfxSlaveObj = thrusterTrailVfxSlaveCD.InstActor(position, rot);
                thrusterTrailVfxSlaveObj.Init(gameObject);
            }
        }

        public void OnShoot(Vector3 speed, ObjectSearchTgt tgt, ObjectSearchTgt shooter, int teamId, int shooterId, int firingId = -1)
        {
            rBody.linearVelocity = speed;
            ld.shooterId = shooterId;
            teamID = teamId;
            ld.target = tgt;
            _positionList[0] = _positionList[1] = pos;
            this.firingId = firingId;
            DrawLine();
        }

        public override void RunBeforePhysics()
        {
            base.RunBeforePhysics();
            ThrustExe();
            ThrustTrailExe();
        }

        public override void RunAfterPhysics()
        {
            base.RunAfterPhysics();
        }

        public override void RunOnAfterFixedUpdateAndAnimation()
        {
            base.RunOnAfterFixedUpdateAndAnimation();
            _positionList[1] = _positionList[0];
            _positionList[0] = pos;
            if (ld.cd.StartHittingFrame <= ld.ExeFrameCount)
            {
                if (ld.cd.useLineHitting)
                {
                    LineHitting();
                }
                if (ld.cd.useProximityFuse && ld.cd.proximityFuseStartFrame < ld.ExeFrameCount)
                {
                    ProximityFuzeHitting();
                }
                if (ld.cd.usePredictionImpactFrameFuse)
                {
                    PredictionImpactFrameHitting();
                }
                var hitPos = HitExe();
                if (hitPos != null) _positionList[0] = hitPos.Value;
            }
            DrawLine();
        }

        public override void OnDotonExe()
        {
            OnHit(null, pos, Vector3.up, HitType.DirectHit);
        }

        private readonly RaycastHit[] _raycatsHits = new RaycastHit[10];
        private void LineHitting()
        {
            var direction = _positionList[0] - _positionList[1];
            var ray = new Ray(_positionList[1], direction.normalized);
            if (Physics.RaycastNonAlloc(
                    ray,
                    _raycatsHits,
                    direction.magnitude,
                    layerOfGround + layerOfMachine + layerOfMissile + layerOfMine + layerOfAerialSmallObject + layerOfShield,
                    QueryTriggerInteraction.Collide) > 0)
            {
                var dist = _hitInfo.HasValue ? (_hitInfo.Value.hitPos - _positionList[1]).magnitude : float.MaxValue;
                foreach (var rh in _raycatsHits)
                {
                    if (!rh.collider || ld.hd.rBody == rh.rigidbody || rh.distance > dist) continue;
                    dist = rh.distance;
                    OnHit(rh.collider, rh.point, rh.normal, HitType.DirectHit);
                }
                for (int i = 0; i < _raycatsHits.Length; i++)
                {
                    _raycatsHits[i] = new RaycastHit();
                }
            }
        }

        private void ProximityFuzeHitting()
        {
            var direction = rBody.linearVelocity;
            var distance = direction.magnitude / 60 * ld.cd.proximityFuseLength;
            var origin = pos;
            if (Physics.SphereCastNonAlloc(
                    origin,
                    ld.cd.proximityFuseRadius,
                    direction.normalized,
                    _raycatsHits,
                    distance,
                    layerOfMachine + layerOfMissile + layerOfMine + layerOfAerialSmallObject + layerOfShield) > 0)
            {
                Collider hitCollider = null;
                if (ld.target != null)
                {
                    foreach (var rh in _raycatsHits)
                    {
                        if (!rh.collider || ld.target.hardBase.rigidBody != rh.rigidbody) continue;
                        hitCollider = rh.collider;
                        break;
                    }
                }
                else
                {
                    foreach (var rh in _raycatsHits)
                    {
                        if (!rh.collider || ld.hd.rBody == rh.rigidbody) continue;
                        hitCollider = rh.collider;
                        break;
                    }
                }
                if (hitCollider is not null) OnHit(hitCollider, pos, Vector3.up, HitType.ProximityFuse);
                for (int i = 0; i < _raycatsHits.Length; i++)
                {
                    _raycatsHits[i] = new RaycastHit();
                }
            }
        }

        private void PredictionImpactFrameHitting()
        {
            if (!ld.target) return;
            var selfV = rigidBody.linearVelocity;
            var tgtV = ld.target.hardBase?.rigidBody.linearVelocity ?? Vector3.zero;
            var relativeV = tgtV - selfV;
            var distance = ld.target.pos - pos;
            relativeV = Vector3.Project(relativeV, distance.normalized);
            var impactFrame = distance.magnitude / relativeV.magnitude * 60;
            if (impactFrame < ld.cd.predictionImpactFrame)
            {
                var tgtCol = ld.target.hardBase?.colliderList.Count > 0 ? ld.target.hardBase.colliderList[0] : null;
                OnHit(tgtCol, pos, -selfV.normalized, HitType.ProximityFuse);
            }
        }

        //private void OnTriggerStay(Collider other)
        //{
        //    if (ld.cd.StartHittingFrame > ld.ExeFrameCount) return;
        //    OnHit(other, other.ClosestPoint(pos), HitType.DirectHit);
        //}

        public void OnHit(Collider col, Vector3 hitPoint, Vector3 hitPointNormal, HitType hitType)
        {
            if (ld.isHit) return;
            _hitInfo = (col, hitPoint, hitPointNormal, hitType);
        }

        /// <summary>
        /// ヒットの処理実行。
        /// TriggerとRayを組み合わせて判定するとき、Triggerの方が先に判定されるので、弾の先端にTriggerがある都合上普通に判定すると先端だけ早く判定されてしまう。
        /// これだと、地表にいる標的を撃つ時、標的を通り抜けて地面に当たってしまう。
        /// そのため、_hitInfoに判定情報をキャッシュして最後に判定を行う。
        /// </summary>
        private Vector3? HitExe()
        {
            if (ld.isHit || !_hitInfo.HasValue)
            {
                _hitInfo = null;
                return null;
            }
            IHaveHitCollider hardBase = null;
            if (_hitInfo.Value.hitCollider != null)
            {
                hardBase = ACM.GetHardFromCollider(_hitInfo.Value.hitCollider);
                if (hardBase is not null && (hardBase.uniqueID == ld.shooterId || hardBase.firingId == firingId)) return null;
            }
            var hitPos = _hitInfo.Value.hitPos;
            ld.OnHit(hardBase, _hitInfo.Value.hitPos, _hitInfo.Value.hitPositionNormal, _hitInfo.Value.hitType, rBody.linearVelocity);
            _hitInfo = null;
            return hitPos;
        }

        private void DrawLine()
        {
            if (lineRenderer == null) return;
            if (!lineRenderer.enabled) lineRenderer.enabled = true;
            lineRenderer.SetPositions(_positionList);
        }

        private void ThrustExe()
        {
            if (thrusterVfxControl == null) return;
            if (ld.cd.AcceleEndFrame < ld.ExeFrameCount)
            {
                thrusterVfxControl.VfxStopImmediately();
            }
            else
            {
                thrusterVfxControl.ThrusterExe(ld.cd.GetThrustPower(ld.ExeFrameCount));
            }
        }

        private void ThrustTrailExe()
        {
            if (thrusterTrailVfxController == null) return;
            var thrustPower = ld.cd.GetThrustPower(ld.ExeFrameCount);
            var position = thrusterVfxControl != null ? thrusterVfxControl.pos : pos;
            thrusterTrailVfxSlaveObj.transform.SetPositionAndRotation(position, rot);
            if (ld.cd.AcceleEndFrame < ld.ExeFrameCount)
            {
                thrusterTrailVfxController.VfxStop();
            }
            else
            {
                thrusterTrailVfxController.EffectExe(thrustPower);
            }
        }

        public override void AddDamage(int penetrationDamage, int impactDamage, int heatDamage, Vector3 impactPoint, Vector3 impactVelocity)
        {
            base.AddDamage(penetrationDamage, impactDamage, heatDamage, impactPoint, impactVelocity);
            ld.damage += penetrationDamage + impactDamage + heatDamage;
            if (ld.damage >= ld.cd.BulletHealthPoint)
            {
                ld.OnHit(null, rBody.position, Vector3.up, HitType.DirectHit, rigidBody.linearVelocity);
            }
            rBody.AddForce(impactDamage * impactVelocity.normalized / ld.cd.baseImpactResistRate);
        }

        public override void Disable()
        {
            base.Disable();
            if (lineRenderer != null) lineRenderer.enabled = false;
            _thrusterTrailVfxController = null;
            thrusterTrailVfxSlaveObj = null;
        }
    }
}