using clrev01.ClAction.Bullets;
using clrev01.ClAction.ObjectSearch;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Serialization;

namespace clrev01.ClAction.Bullets
{
    [CreateAssetMenu(menuName = "CommonData/Mine/AirTurretDroneCD")]
    public class AirTurretDroneCD : MineCD<AirTurretDroneCD, AirTurretDroneLD, AirTurretDroneHD>
    {
        public float normalAltitude = 50;
        public float stayAirThrustRatio = 1;
        public float toTargetThrustRatio = 1;
        public float obstacleAvoidThrustRatio = 3;
        public float obstacleAvoidDistanceRatio = 1;
        public float sideMoveRate = 0.1f;
        public float thrustMaxSpeed = 1000;
        public float thrustGain = 3;
        public AnimationCurve thrustAcceleCurve = new();
        public float fireStartDistance = 300;
        public BulletCD origBullet;
        public float ammoNum = 24;
        public SearchParameterData searchParameterData = new();
    }
}