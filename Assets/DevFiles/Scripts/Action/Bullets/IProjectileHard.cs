using clrev01.ClAction.ObjectSearch;
using UnityEngine;

namespace clrev01.ClAction.Bullets
{
    public interface IProjectileHard
    {
        public abstract IProjectileCommonData projectileCommonData { get; }
        public void OnShoot(Vector3 speed, ObjectSearchTgt tgt, ObjectSearchTgt shooter, int teamId, int shooterId, int hitIgnoreId = -1);
    }
}