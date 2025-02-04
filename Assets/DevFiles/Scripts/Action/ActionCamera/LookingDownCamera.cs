using clrev01.Bases;
using clrev01.ClAction.Machines;
using UnityEngine;

namespace clrev01.ClAction.ActionCamera
{
    public class LookingDownCamera : ActionCameraBase
    {
        [SerializeField]
        private float minCameraDistanceExistAimTgt = 20;
        [SerializeField]
        private Vector2 defaultCameraRotation;
        [SerializeField]
        private float cameraMoveRatio = 0.75f;
        private Vector2 cameraRotation
        {
            get => StaticInfo.Inst.ActionSettingData.lookingDownCameraRotation ?? defaultCameraRotation;
            set => StaticInfo.Inst.ActionSettingData.lookingDownCameraRotation = value;
        }
        protected override float cameraDistance
        {
            get
            {
                if (cameraTgt?.ld.latestAimTgt == null) return StaticInfo.Inst.ActionSettingData.cameraDistance2;
                var dist = Vector3.Distance(cameraTgt.ld.latestAimTgt.pos, cameraTgt.pos);
                var nowCamDist = -actCam.localPosition.z;
                var screenShortEdge = CalculateScreenShortEdge(nowCamDist);
                return Mathf.Max(nowCamDist * dist / screenShortEdge * 1.1f, minCameraDistanceExistAimTgt);
            }
            set => StaticInfo.Inst.ActionSettingData.cameraDistance2 = value;
        }
        protected override bool useObstacleAvoidance => false;
        /// <summary>
        /// ロックオンターゲット
        /// </summary>
        private Vector3 lookPos
        {
            get
            {
                var tgtPos = cameraTgt.pos;
                if (cameraTgt.ld.latestAimTgt == null) return tgtPos;
                tgtPos += cameraTgt.ld.latestAimTgt.pos;
                tgtPos /= 2;
                return tgtPos;
            }
        }


        public override void OnDrug(Vector2 drugV)
        {
            base.OnDrug(drugV);
            var cameraRot = cameraRotation;
            cameraRot += drugV * cameraMoveRatio;
            cameraRot.y = Mathf.Clamp(cameraRot.y, 0, 90);
            cameraRotation = cameraRot;
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            actCameObj.rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0);
            UpdateRotateCamera(lookPos, Vector3.up);
        }
        public override void CameraReset()
        {
            cameraRotation = defaultCameraRotation;
        }

        // メソッド: カメラからの距離を引数に取り、短辺の長さを返す
        private float CalculateScreenShortEdge(float distanceFromCamera)
        {
            // カメラのフィールドオブビューとアスペクト比を取得
            var cam = Camera.main;
            var verticalFOV = cam.fieldOfView;
            var aspectRatio = cam.aspect;

            // 垂直FOVをラジアンに変換
            var verticalFOVRad = verticalFOV * Mathf.Deg2Rad;

            // 距離からスクリーンの高さを計算
            var screenHeight = 2 * distanceFromCamera * Mathf.Tan(verticalFOVRad / 2);

            // 高さから幅を計算
            var screenWidth = screenHeight * aspectRatio;

            // 短辺を計算（高さと幅のどちらが短いかを比較）
            var shortEdge = Mathf.Min(screenHeight, screenWidth);

            // 短辺の長さを返す
            return shortEdge;
        }
    }
}