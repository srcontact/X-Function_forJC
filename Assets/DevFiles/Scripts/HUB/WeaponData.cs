using clrev01.Bases;
using clrev01.ClAction.Bullets;
using clrev01.ClAction.Machines.AdditionalTurret;
using clrev01.Extensions;
using UnityEngine;

namespace clrev01.HUB
{
    [System.Serializable]
    public class WeaponData : HubData
    {
        public string name;
        public IProjectileCommonData bulletCD => bulletCdReference.GetAsset(StaticInfo.Inst.gameObject);
        [SerializeField]
        private AssetReferenceSet<IProjectileCommonData> bulletCdReference;
        public int defaultAmoNum = 100;
        public int maxAmoNum = 1000;
        [SerializeField]
        private ComponentReferenceSet<AdditionalTurretObj> additionalTurretObjReference;
        public AdditionalTurretObj GetAdditionalTurretObj()
        {
            return additionalTurretObjReference.IsNotSetAsset() ? null : additionalTurretObjReference?.GetInstanceUsePool(out _);
        }
    }
}