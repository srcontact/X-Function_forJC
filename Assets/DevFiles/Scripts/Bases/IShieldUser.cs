using clrev01.ClAction.Shield;
using UnityEngine;

namespace clrev01.Bases
{
    public interface IShieldUser
    {
        public ShieldCD shieldCd { get; }
        public void AddShieldDamage(int damage, Vector3 impactV, float shieldSize);
    }
}