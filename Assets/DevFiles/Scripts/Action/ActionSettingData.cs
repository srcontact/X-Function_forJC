using clrev01.ClAction.ActionCamera;
using clrev01.Save;
using MemoryPack;
using UnityEngine;
using UnityEngine.Serialization;

namespace clrev01.ClAction
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class ActionSettingData
    {
        public CameraMode cameraMode;
        public Vector2? normalCameraRotation = null;
        public Vector2? lockOnTgtGazeCameraPosition = null;
        public float cameraDistance1 = 20;
        public bool cameraObstacleAvoidanceMode = true;
        public FrameParSec frameParSec = FrameParSec.@default;
        public SpeedUnitType actionUiSpeedUnitType = SpeedUnitType.MeterPerSecond;
        public Vector2? lookingDownCameraRotation = null;
        public float cameraDistance2 = 20;
    }
}