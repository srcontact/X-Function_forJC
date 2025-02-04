using clrev01.Bases;
using clrev01.ClAction.ObjectSearch;
using UnityEngine;

namespace clrev01.ClAction.Bullets
{
    public abstract class MineLD<C, L, H> : LocalData<C, L, H>, IProjectileLocalData
        where C : MineCD<C, L, H>
        where L : MineLD<C, L, H>, new()
        where H : MineHD<C, L, H>
    {
        public int shooterId { get; set; } = -1;
        public ObjectSearchTgt target { get; set; }
        [SerializeField]
        public bool isHit { get; set; }
        [SerializeField]
        public Vector3 accele { get; set; }
        [SerializeField]
        public Vector3 jumpV { get; set; }
        public MineMoveState moveState;
        public int actionState;
        public bool isGrounded = false;
        public int explosionCount = 0;
        public int Damage { get; set; } = 0;

        public override void ResetEveryFrame()
        {
            base.ResetEveryFrame();
            accele = Vector3.zero;
            jumpV = Vector3.zero;
            hd.objectSearchTgt.jammingSize = 0;
            hd.objectSearchTgt.jammedSize = 0;
        }

        public override void RunBeforePhysics()
        {
            base.RunBeforePhysics();
            if (target != null && !target.gameObject.activeSelf) target = null;
        }

        public void OnHit(IHaveHitCollider hitHard, Vector3 hitPos, Vector3 hitPostionNorml, HitType hitType, Vector3 hitEffectVector)
        {
            isHit = true;
            if (hitType == HitType.DirectHit)
            {
                var damage = cd.directHitPower.GetPower(1, 1, 1, 1);
                var impactVelocity = (hd.pos - hitPos).normalized;
                hitHard?.AddDamage(damage.penetrationPower, damage.impactPower, damage.heatPower, hitPos, impactVelocity);
            }
            foreach (var spawnOnHitInfo in cd.SpawnOnHitInfos)
            {
                spawnOnHitInfo.SpawnExe(hitPos, hitEffectVector, hitPostionNorml, hitHard is HardBase @base ? @base.objectSearchTgt : null, hd.objectSearchTgt, hitType);
            }
        }
    }
}