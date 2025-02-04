using UnityEngine;

namespace clrev01.ClAction.Machines.Motion
{
    [System.Serializable]
    public class FightHomingData
    {
        public float homingStartLength = 50;
        [Range(0f, 360f)]
        public float homingHorizontalAngle = 90;
        [Range(-180f, 180f)]
        public float homingHorizontalAngleOffset = 0;
        [Range(0f, 360f)]
        public float homingVerticalAngle = 90;
        [Range(-180f, 180f)]
        public float homingVerticalAngleOffset = 0;
        public float groundedVelocityFixMaxSpeed = 1000, inAirVelocityFixMaxSpeed = 500;
    }
}