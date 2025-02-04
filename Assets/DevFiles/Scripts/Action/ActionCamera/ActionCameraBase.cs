using System;
using clrev01.Bases;
using clrev01.ClAction.Machines;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.ActionCamera
{
    public abstract class ActionCameraBase : BaseOfCL
    {
        /// <summary>
        /// カメラの親オブジェクトのTransform
        /// </summary>
        public Transform actCameObj;
        /// <summary>
        /// カメラのオブジェクトのTransform
        /// </summary>
        public Transform actCam;
        /// <summary>
        /// カメラターゲットオブジェクト
        /// </summary>
        protected MachineHD cameraTgt => ACM.cameraTgtMachine >= 0 && ACM.cameraTgtMachine < ACM.machineList.Count ? ACM.machineList[ACM.cameraTgtMachine] : null;
        [SerializeField, Range(0f, 1f)]
        private float actCamRotateLerp = 0.8f;
        [SerializeField, Range(1, 360)]
        protected float actCamRotateMaxAngle = 12;
        /// <summary>
        /// カメラ距離
        /// </summary>
        protected abstract float cameraDistance { get; set; }
        /// <summary>
        /// カメラ最近距離
        /// </summary>
        public float minCameraDistance = 5;
        /// <summary>
        /// カメラ最遠距離
        /// </summary>
        public float maxCameraDistance = 100;
        [SerializeField, Range(0, 1)]
        private float obstacleAvoidanceLerpPar = 0.1f;
        protected abstract bool useObstacleAvoidance { get; }
        private float _distanceLog;

        private void Start()
        {
            _distanceLog = cameraDistance;
        }

        public virtual void OnDrug(Vector2 drugV)
        { }

        public void ScrollMove(float ratio)
        {
            cameraDistance = Mathf.Clamp(cameraDistance * ratio, minCameraDistance, maxCameraDistance);
        }

        public virtual void OnUpdate()
        {
            actCameObj.position = cameraTgt.pos;
            UpdateDistance();
        }

        private void UpdateDistance()
        {
            if (useObstacleAvoidance && StaticInfo.Inst.ActionSettingData.cameraObstacleAvoidanceMode)
            {
                var dist = cameraDistance;
                if (Physics.Raycast(actCameObj.position, actCameObj.TransformDirection(Vector3.back), out var hitInfo, cameraDistance, layerOfGround))
                {
                    dist = hitInfo.distance;
                }
                _distanceLog = Mathf.Lerp(_distanceLog, dist, obstacleAvoidanceLerpPar);
            }
            else _distanceLog = cameraDistance;
            actCam.localPosition = Vector3.back * _distanceLog;
        }

        protected void UpdateRotateCamera(Vector3 tgt, Vector3 upwardV)
        {
            var rotate = UtlOfCL.LookRotationCheckViewingVectorIsZero(tgt - actCam.position, upwardV);
            actCam.rotation = Quaternion.RotateTowards(actCam.rotation, Quaternion.Lerp(actCam.rotation, rotate, actCamRotateLerp), actCamRotateMaxAngle);
        }

        public abstract void CameraReset();
    }
}