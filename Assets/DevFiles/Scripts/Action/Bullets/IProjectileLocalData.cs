using clrev01.Bases;
using clrev01.ClAction.ObjectSearch;
using UnityEngine;

namespace clrev01.ClAction.Bullets
{
    public interface IProjectileLocalData
    {
        public int shooterId { get; set; }
        public ObjectSearchTgt target { get; set; }
        public bool isHit { get; set; }
        public Vector3 accele { get; set; }
        public Vector3 jumpV { get; set; }


        public void OnHit(IHaveHitCollider hitHard, Vector3 hitPos, Vector3 hitPointNormal, HitType hitType, Vector3 hitEffectVector = new());
    }
}