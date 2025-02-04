using clrev01.Bases;
using Den.Tools;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Bullets
{
    public class HitEffectHD : Hard<HitEffectCD, HitEffectLD, HitEffectHD>
    {

        public override void RunOnAfterFixedUpdateAndAnimation()
        {
            base.RunOnAfterFixedUpdateAndAnimation();
            if (ld.cd.IsExplosionHit)
            {
                ExplosionHitting();
            }
        }

        private readonly Collider[] _hitColliders = new Collider[10];
        private void ExplosionHitting()
        {
            _hitColliders.Fill(null);
            Physics.OverlapSphereNonAlloc(
                pos,
                ld.GetRadius(),
                _hitColliders,
                layerOfMachine + layerOfMissile + layerOfMine + layerOfAerialSmallObject + layerOfShield
            );
            foreach (var hc in _hitColliders)
            {
                if (ld.alreadyHits.Contains(hc) || ld.alreadyHits[^1] != null) continue;
                for (var i = 0; i < ld.alreadyHits.Length; i++)
                {
                    if (ld.alreadyHits[i] != null) continue;
                    ld.alreadyHits[i] = hc;
                    break;
                }
                if (hc.attachedRigidbody != null)
                {
                    var toHcRb = hc.attachedRigidbody.position - pos;
                    if (Physics.Raycast(pos, toHcRb.normalized, out var raycastHit, toHcRb.magnitude, layerOfGround + layerOfShield) && raycastHit.collider != hc) return;
                }
                var hitHds = ACM.GetHardFromCollider(hc);
                hitHds?.AddDamage(ld.cd.ExplosionPower, 0, rigidBody != null ? rigidBody.linearVelocity : Vector3.zero, pos, ld.cd.durationFrame, ld.ExeFrameCount);
            }
        }
        public override void OnDotonExe()
        { }
    }
}