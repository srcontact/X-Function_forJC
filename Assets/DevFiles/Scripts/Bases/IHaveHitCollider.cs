using clrev01.ClAction;
using UnityEngine;

namespace clrev01.Bases
{
    public interface IHaveHitCollider
    {
        public int uniqueID { get; set; }
        public int firingId { get; set; }
        public void RegisterHitColliders();
        public void AddDamage(PowerPar bulletPow, float baseSpeed, Vector3 speed, Vector3 impactPos, int maxFrame, int exeFrame);

        public void AddDamage(int penetrationDamage, int impactDamage, int heatDamage, Vector3 impactPoint, Vector3 impactVelocity);
    }
}