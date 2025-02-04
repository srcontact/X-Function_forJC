using clrev01.ClAction.Shield;
using clrev01.Extensions;

namespace clrev01.HUB
{
    [System.Serializable]
    public class ShieldData : HubData
    {
        public string name;
        public ShieldCD shieldCd => shieldCdReference.GetAsset();
        public AssetReferenceSet<ShieldCD> shieldCdReference;
    }
}