using clrev01.Programs.FieldPar;
using UnityEngine;

namespace clrev01.ClAction.Bullets
{
    [CreateAssetMenu(menuName = "CommonData/Mine/ProximityFuseMineCD")]
    public class ProximityFuseMineCD : MineCD<ProximityFuseMineCD, ProximityFuseMineLD, ProximityFuseMineHD>
    {
        public SphereSearchFieldParForInspector proximityFuseRange = new()
        {
            farRadius = 50,
        };
        public int explosionNum = 1;
        public int searchIntervalFrame = 30;
    }
}