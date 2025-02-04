using clrev01.ClAction.ObjectSearch;
using clrev01.Programs.FieldPar;
using UnityEngine;

namespace clrev01.ClAction.Bullets
{
    [CreateAssetMenu(menuName = "CommonData/Mine/JumpMineCD")]
    public class JumpMineCD : MineCD<JumpMineCD, JumpMineLD, JumpMineHD>
    {
        public SphereSearchFieldParForInspector proximityFuseRange = new()
        {
            farRadius = 100,
            horizontalAngle = 360,
            verticalAngle1 = 90,
            verticalAngle2 = -90,
        };
        public float maxTrackingDistance = 200;
        public int searchIntervalFrame = 30;
        public SearchParameterData searchParameterData = new();

        public float jumpPow = 200;
        public float normalJumpAngle = 20;
        public int jumpInterval = 10;
    }
}