using System;
using clrev01.Bases;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Bullets
{
    public class ProjectileDirectHitting : BaseOfCL
    {
        private int _spawnFrame;
        private int ExeFrameCount => ActionManager.Inst.actionFrame - _spawnFrame;
        [SerializeField]
        private IProjectileHard projectileHard;
        private (Collider hitCollider, Vector3 hitPos, Vector3 hitPointNormal)? _hitInfo;


        public void Init(int spawnFrame)
        {
            _spawnFrame = spawnFrame;
        }

        /// <summary>
        /// ヒットの処理実行。
        /// TriggerとRayを組み合わせて判定するとき、Triggerの方が先に判定されるので、弾の先端にTriggerがある都合上普通に判定すると先端だけ早く判定されてしまう。
        /// これだと、地表にいる標的を撃つ時、標的を通り抜けて地面に当たってしまう。
        /// そのため、_hitInfoに判定情報をキャッシュして最後に判定を行う。
        /// </summary>
        public void HitExe(IProjectileLocalData ld, Rigidbody rBody, IReadOnlyCollection<Collider> colliderList, int shooterId, int firingId)
        {
            LineHitting(rBody, colliderList);

            if (!_hitInfo.HasValue) return;
            if (_hitInfo.Value.hitCollider == null) return;
            var hardBase = ACM.GetHardFromCollider(_hitInfo.Value.hitCollider);
            if (hardBase is not null && (hardBase.uniqueID == shooterId || hardBase.firingId == firingId)) return;
            pos = _hitInfo.Value.hitPos;
            ld.OnHit(hardBase, _hitInfo.Value.hitPos, _hitInfo.Value.hitPointNormal, HitType.DirectHit, rBody.linearVelocity);
            _hitInfo = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (projectileHard.projectileCommonData.StartHittingFrame > ExeFrameCount || ((1 << other.gameObject.layer) & projectileHard.projectileCommonData.HitTgtLayer) == 0) return;
            var closestPoint = other.ClosestPoint(pos);
            OnHit(other, closestPoint, Vector3.Normalize(pos - other.transform.position));
        }

        private void OnCollisionEnter(Collision other)
        {
            if (projectileHard.projectileCommonData.StartHittingFrame > ExeFrameCount || ((1 << other.gameObject.layer) & projectileHard.projectileCommonData.HitTgtLayer) == 0) return;
            var closestPoint = other.collider.ClosestPoint(pos);
            OnHit(other.collider, closestPoint, other.contacts[0].normal);
        }

        private void LineHitting(Rigidbody rBody, IReadOnlyCollection<Collider> colliderList)
        {
            if (rBody.linearVelocity.sqrMagnitude == 0) return;
            var direction = rBody.linearVelocity / 60;
            var ray = new Ray(pos - direction, direction.normalized);

            if (!Physics.Raycast(
                    ray,
                    out var hit,
                    direction.magnitude,
                    projectileHard.projectileCommonData.HitTgtLayer,
                    QueryTriggerInteraction.Collide)
               ) return;
            if (colliderList.Count > 0)
            {
                foreach (var x in colliderList)
                {
                    if (hit.collider == x) continue;
                    OnHit(hit.collider, hit.point, hit.normal);
                    break;
                }
            }
            else
            {
                OnHit(hit.collider, hit.point, hit.normal);
            }
        }

        private void OnHit(Collider collider, Vector3 hitPoint, Vector3 hitPointNormal)
        {
            _hitInfo = (collider, hitPoint, hitPointNormal);
        }

        private void OnValidate()
        {
            projectileHard = GetComponent<IProjectileHard>();
        }
    }
}