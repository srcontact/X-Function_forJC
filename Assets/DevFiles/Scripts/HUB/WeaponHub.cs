using clrev01.ClAction.Bullets;
using clrev01.ClAction.Machines.AdditionalTurret;
using UnityEngine;

namespace clrev01.HUB
{
    [CreateAssetMenu(menuName = "Hub/WeaponHub")]
    public class WeaponHub : HubBase<WeaponData>
    {
        public Sprite nullIcon, blankIcon;


        public IProjectileCommonData GetBulletCD(int code)
        {
            var w = GetData(code);
            return w == null ? datas[0].bulletCD : w.bulletCD;
        }
        public string GetBulletName(int code)
        {
            var w = GetData(code);
            return w == null ? datas[0].name : w.name;
        }
        public int GetGlobalDefaultAmoNum(int code)
        {
            var w = GetData(code);
            return w?.defaultAmoNum ?? 0;
        }
        public int GetGlobalMaxAmoNum(int code)
        {
            var w = GetData(code);
            return w?.maxAmoNum ?? 0;
        }
        public AdditionalTurretObj GetAdditionalTurretObj(int code)
        {
            var w = GetData(code);
            return w?.GetAdditionalTurretObj();
        }

        private void OnValidate()
        { }
    }
}