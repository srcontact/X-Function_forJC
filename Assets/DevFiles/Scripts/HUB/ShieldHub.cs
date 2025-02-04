using clrev01.ClAction.Shield;
using clrev01.Save;
using UnityEngine;

namespace clrev01.HUB
{
    [CreateAssetMenu(menuName = "Hub/ShieldHub")]
    public class ShieldHub : HubBase<ShieldData>
    {
        public ShieldCD GetShieldPar(int code)
        {
            return GetData(code)?.shieldCd;
        }

        public string GetShieldName(int code)
        {
            return GetData(code)?.name;
        }

        public (float radiusMin, float radiusMax, float offsetMin, float offsetMax) GetShieldMinMax(int code, CoordinateSystemType coordinateSystemType)
        {
            return GetShieldPar(code).GetShieldMinMax(coordinateSystemType);
        }
    }
}