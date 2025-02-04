using RootMotion.FinalIK;
using UnityEngine;

namespace clrev01.ClAction.Machines.Motion
{
    public class CCDIKTest1 : MonoBehaviour
    {
        public IK legIK;
        public Vector3 pos;
        public bool setPos, inUpdate;
        public AnimationCurve hSpeed, vSpeed;
        public int loopFrame = 20, count = 0;

        public void Update()
        {
            Move();
        }
        public void Move()
        {
            legIK.GetIKSolver().SetIKPosition(pos);
        }
    }
}