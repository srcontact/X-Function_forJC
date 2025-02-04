using clrev01.Bases;
using clrev01.ClAction.Machines;
using UnityEngine;

namespace clrev01.ClAction.ActionCamera
{
    public class FollowingCamera : ActionCameraBase
    {
        /// <summary>
        /// 通常カメラ移動レシオ
        /// </summary>
        public float normalCameraMoveRatio = 0.75f;
        /// <summary>
        /// カメラ初期回転
        /// </summary>
        public Vector2 defaultNormalCameraRotation = new(0, 20);
        /// <summary>
        /// 通常カメラ回転値
        /// </summary>
        private Vector2 normalCameraRotation
        {
            get => StaticInfo.Inst.ActionSettingData.normalCameraRotation ?? defaultNormalCameraRotation;
            set => StaticInfo.Inst.ActionSettingData.normalCameraRotation = value;
        }
        /// <summary>
        /// 通常カメラ回転
        /// </summary>
        protected Quaternion tgtRot;
        protected override float cameraDistance
        {
            get => StaticInfo.Inst.ActionSettingData.cameraDistance1;
            set => StaticInfo.Inst.ActionSettingData.cameraDistance1 = value;
        }
        protected override bool useObstacleAvoidance => true;


        public override void OnDrug(Vector2 drugV)
        {
            base.OnDrug(drugV);
            var cameraRot = normalCameraRotation;
            cameraRot += drugV * normalCameraMoveRatio;
            cameraRot.y = Mathf.Clamp(cameraRot.y, 0, 90);
            normalCameraRotation = cameraRot;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (cameraTgt.ld.movePar.moveState != CharMoveState.uncontrollable)
            {
                var fAngle = Mathf.Abs(90 - Vector3.Angle(cameraTgt.transform.forward, Vector3.up));
                var rAngle = Mathf.Abs(90 - Vector3.Angle(cameraTgt.transform.right, Vector3.up));
                var forward = rAngle < fAngle ? Vector3.Cross(cameraTgt.transform.right, Vector3.up) : Quaternion.Euler(0, 90, 0) * Vector3.Cross(cameraTgt.transform.forward, Vector3.up);
                tgtRot = Quaternion.LookRotation(forward, Vector3.up);
            }
            actCameObj.rotation = tgtRot * Quaternion.Euler(normalCameraRotation.y, normalCameraRotation.x, 0);
            UpdateRotateCamera(cameraTgt.pos, actCameObj.up);
        }

        public override void CameraReset()
        {
            normalCameraRotation = defaultNormalCameraRotation;
        }
    }
}