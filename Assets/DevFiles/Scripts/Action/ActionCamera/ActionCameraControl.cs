using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.ClAction.ObjectSearch;
using clrev01.Menu;
using System;
using System.Collections.Generic;
using EnumLocalizationWithI2Localization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.ActionCamera
{
    public class ActionCameraControl : BaseOfCL
        , IDragHandler
        , IBeginDragHandler
        , IEndDragHandler
        , IScrollHandler
    {
        /// <summary>
        /// カメラターゲットを一つ前に変更するボタン
        /// </summary>
        public MenuButton cameraTgtPrevious;
        /// <summary>
        /// カメラターゲットを一つ次に変更するボタン
        /// </summary>
        public MenuButton cameraTgtNext;
        /// <summary>
        /// カメラポジションをリセットするボタン
        /// </summary>
        public MenuButton cameraTgtReset;
        /// <summary>
        /// カメラモード変更ボタン
        /// </summary>
        public MenuButton cameraModeChangeButton;
        /// <summary>
        /// カメラモード変更ボタンのテキスト
        /// </summary>
        public TextMeshProUGUI cameraModeButtonText;

        /// <summary>
        /// カメラターゲット追跡カメラ
        /// </summary>
        public FollowingCamera followingCamera;
        /// <summary>
        /// ターゲット追跡カメラ
        /// </summary>
        public LockOnTgtGazeCamera lockOnTgtGazeCamera;
        /// <summary>
        /// 見下ろしカメラ
        /// </summary>
        public LookingDownCamera lookingDownCamera;
        private ActionCameraBase actionCamera =>
            (cameraMode, existLockOnTgt) switch
            {
                (cameraMode: CameraMode.Following, _) => followingCamera,
                (cameraMode: CameraMode.LockOnTgtGaze, false) => followingCamera,
                (cameraMode: CameraMode.LockOnTgtGaze, true) => lockOnTgtGazeCamera,
                (cameraMode: CameraMode.LookingDown, _) => lookingDownCamera,
                _ => throw new ArgumentOutOfRangeException()
            };

        /// <summary>
        /// 直前のドラッグ座標
        /// </summary>
        private Vector2 _currentDrugPos;
        /// <summary>
        /// カメラスケーリングスクロールレシオ
        /// </summary>
        public float scalingScrollRatio = 1;
        /// <summary>
        /// タッチスケール変更中フラグ
        /// </summary>
        private bool _doTouchScaling;
        /// <summary>
        /// タッチスケール変更スタート時ピンチ距離
        /// </summary>
        private float _touchStartPinchDist;
        /// <summary>
        /// カメラ回転モード
        /// </summary>
        private CameraMode cameraMode
        {
            get => StaticInfo.Inst.ActionSettingData.cameraMode;
            set
            {
                StaticInfo.Inst.ActionSettingData.cameraMode = value;
                cameraModeButtonText.SetText(cameraMode.ToLocalizedString());
            }
        }
        /// <summary>
        /// カメラターゲットオブジェクト
        /// </summary>
        private MachineHD cameraTgt => ACM.machineList[ACM.cameraTgtMachine];
        /// <summary>
        /// ロックオンターゲット
        /// </summary>
        private ObjectSearchTgt lockOnTgt => cameraTgt.ld.latestAimTgt;
        /// <summary>
        /// ロックオンターゲット注視モードがアクティブか否か
        /// </summary>
        private bool existLockOnTgt => lockOnTgt != null || _lockOnTgtLog.Count > 0;

        private List<(ObjectSearchTgt tgt, int latestFrame, Vector3 pos)> _lockOnTgtLog = new();


        public void OnBeginDrag(PointerEventData eventData)
        {
            _currentDrugPos = Input.mousePosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_doTouchScaling) return;
            Vector2 nowDrugPos = Input.mousePosition;
            Vector2 drugV = nowDrugPos - _currentDrugPos;
            drugV.y = -drugV.y;
            _currentDrugPos = nowDrugPos;
            actionCamera.OnDrug(drugV);
        }

        public void OnEndDrag(PointerEventData eventData)
        { }

        public void OnScroll(PointerEventData eventData)
        {
            var scrollWheel = -Input.GetAxisRaw("Mouse ScrollWheel");
            actionCamera.ScrollMove(1 + scrollWheel * scalingScrollRatio);
        }

        private void TouchScaling()
        {
            if (Input.touchCount >= 2)
            {
                Touch t1 = Input.GetTouch(0);
                Touch t2 = Input.GetTouch(1);
                float pinchDist = Vector2.Distance(t1.position, t2.position);
                if (!_doTouchScaling)
                {
                    _doTouchScaling = true;
                    _touchStartPinchDist = pinchDist;
                }
                else
                {
                    actionCamera.ScrollMove(_touchStartPinchDist / pinchDist);
                    _touchStartPinchDist = pinchDist;
                }
            }
            else
            {
                _doTouchScaling = false;
            }
        }

        private void Start()
        {
            cameraTgtPrevious.OnClick.AddListener(() => TgtPrevious());
            cameraTgtNext.OnClick.AddListener(() => TgtNext());
            cameraTgtReset.OnClick.AddListener(() => CameraRotReset());
            cameraModeChangeButton.OnClick.AddListener(() => LockOnTgtGazeModeChange());
            cameraModeButtonText.SetText(cameraMode.ToString());
        }

        private void Update()
        {
            if (ACM.machineList.Count < 1) return;
            TouchScaling();
            actionCamera.OnUpdate();
        }

        public void TgtPrevious()
        {
            ACM.cameraTgtMachine = (int)Mathf.Repeat(ACM.cameraTgtMachine - 1, ACM.machineList.Count);
        }
        public void TgtNext()
        {
            ACM.cameraTgtMachine = (int)Mathf.Repeat(ACM.cameraTgtMachine + 1, ACM.machineList.Count);
        }
        public void CameraRotReset()
        {
            actionCamera.CameraReset();
        }
        private void LockOnTgtGazeModeChange()
        {
            var cameraModes = (CameraMode[])Enum.GetValues(typeof(CameraMode));
            cameraMode = cameraModes[((int)cameraMode + 1) % cameraModes.Length];
        }
    }
}