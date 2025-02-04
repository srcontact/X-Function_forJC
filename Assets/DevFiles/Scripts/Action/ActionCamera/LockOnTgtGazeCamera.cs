using System.Collections.Generic;
using clrev01.Bases;
using clrev01.ClAction.ObjectSearch;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.ActionCamera
{
    public class LockOnTgtGazeCamera : ActionCameraBase
    {
        /// <summary>
        /// ロックオンターゲット注視カメラ移動レシオ
        /// </summary>
        public float lockOnTgtGazeCameraMoveRatio = 0.25f;

        /// <summary>
        /// ロックオンターゲット注視カメラ初期座標
        /// </summary>
        public Vector2 defaultLockOnTgtGazeCameraPosition = Vector3.zero;
        /// <summary>
        /// ロックオンターゲット注視カメラ座標
        /// </summary>
        private Vector2 lockOnTgtGazeCameraPosition
        {
            get => StaticInfo.Inst.ActionSettingData.lockOnTgtGazeCameraPosition ?? defaultLockOnTgtGazeCameraPosition;
            set => StaticInfo.Inst.ActionSettingData.lockOnTgtGazeCameraPosition = value;
        }
        /// <summary>
        /// ロックオンターゲット注視カメラ移動範囲
        /// </summary>
        public Vector2 lockOnTgtGazeCameraPositionClamp = new(5, 5);
        /// <summary>
        /// ロックオンターゲット
        /// </summary>
        private ObjectSearchTgt lockOnTgt => cameraTgt.ld.latestAimTgt;

        private List<(ObjectSearchTgt tgt, int latestFrame, Vector3 pos)> _lockOnTgtLog = new();
        [SerializeField]
        private int lockOnTgtGazeTransitionFrame = 20;
        private (ObjectSearchTgt currentTgt, int firstFrame, Vector3 pos)? _currentLockOnTgt;
        private Vector3 lockOnTgtGazePos
        {
            get
            {
                var v = Vector3.zero;
                var ratioSum = 0;
                if (_currentLockOnTgt.HasValue)
                {
                    var ratio = Mathf.Min(lockOnTgtGazeTransitionFrame, ACM.actionFrame - _currentLockOnTgt.Value.firstFrame);
                    ratio *= ratio;
                    ratioSum += ratio;
                    v += _currentLockOnTgt.Value.pos * ratio;
                }
                for (var i = _lockOnTgtLog.Count - 1; i >= 0; i--)
                {
                    var l = _lockOnTgtLog[i];
                    var ratio = lockOnTgtGazeTransitionFrame - Mathf.Min(lockOnTgtGazeTransitionFrame, ACM.actionFrame - l.latestFrame);
                    ratio *= ratio;
                    ratioSum += ratio;
                    v += l.pos * ratio;
                }
                return v / ratioSum;
            }
        }
        [SerializeField, Range(0f, 1f)]
        private float actCamObjRotateLerp = 0.8f;
        [SerializeField, Range(0f, 1f)]
        private float targetCameraTargetFocusRatio = 0.25f;
        protected override float cameraDistance
        {
            get => StaticInfo.Inst.ActionSettingData.cameraDistance1;
            set => StaticInfo.Inst.ActionSettingData.cameraDistance1 = value;
        }
        protected override bool useObstacleAvoidance => true;
        
        
        public override void OnDrug(Vector2 drugV)
        {
            base.OnDrug(drugV);
            var cameraPos = lockOnTgtGazeCameraPosition;
            cameraPos += drugV * lockOnTgtGazeCameraMoveRatio;
            cameraPos.x =
                Mathf.Clamp(cameraPos.x, -lockOnTgtGazeCameraPositionClamp.x, lockOnTgtGazeCameraPositionClamp.x);
            cameraPos.y =
                Mathf.Clamp(cameraPos.y, -lockOnTgtGazeCameraPositionClamp.y, lockOnTgtGazeCameraPositionClamp.y);
            lockOnTgtGazeCameraPosition = cameraPos;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            LockOnTgtGazePosUpdate();
            var cameraTgtPos = lockOnTgtGazePos * targetCameraTargetFocusRatio + cameraTgt.pos * (1 - targetCameraTargetFocusRatio);
            var lookRotation =
                Quaternion.Euler(Vector3.Scale(LookRotationCheckViewingVectorIsZero(cameraTgtPos - cameraTgt.pos, Vector3.up).eulerAngles, Vector3.up));
            var tgtRot = lookRotation * Quaternion.Euler(lockOnTgtGazeCameraPosition.y, lockOnTgtGazeCameraPosition.x, 0);
            var lerpRot = Quaternion.Lerp(actCameObj.rotation, tgtRot, actCamObjRotateLerp);
            actCameObj.rotation = Quaternion.RotateTowards(actCameObj.rotation, lerpRot, actCamRotateMaxAngle);
            UpdateRotateCamera(cameraTgtPos, Vector3.up);
        }

        private void LockOnTgtGazePosUpdate()
        {
            if (lockOnTgt != null)
            {
                if (!_currentLockOnTgt.HasValue || _currentLockOnTgt.Value.currentTgt != lockOnTgt)
                {
                    if (_currentLockOnTgt.HasValue) _lockOnTgtLog.Add(new(_currentLockOnTgt.Value.currentTgt, ACM.actionFrame, _currentLockOnTgt.Value.pos));
                    var logIndex = _lockOnTgtLog.FindIndex(x => x.tgt == lockOnTgt);
                    if (logIndex != -1)
                    {
                        _currentLockOnTgt = new(lockOnTgt, _lockOnTgtLog[logIndex].latestFrame, lockOnTgt.pos);
                        _lockOnTgtLog.RemoveAt(logIndex);
                    }
                    else _currentLockOnTgt = new(lockOnTgt, ACM.actionFrame, lockOnTgt.pos);
                }
                else
                {
                    var v = _currentLockOnTgt.Value;
                    v.pos = lockOnTgt.pos;
                    _currentLockOnTgt = v;
                }
            }
            else if (_currentLockOnTgt != null)
            {
                _lockOnTgtLog.Add(new(_currentLockOnTgt.Value.currentTgt, ACM.actionFrame, _currentLockOnTgt.Value.pos));
                _currentLockOnTgt = null;
            }

            for (var i = 0; i < _lockOnTgtLog.Count; i++)
            {
                var l = _lockOnTgtLog[i];
                if (l.latestFrame + lockOnTgtGazeTransitionFrame <= ACM.actionFrame)
                {
                    _lockOnTgtLog.RemoveAt(i);
                    continue;
                }
                if (l.tgt != null && l.tgt.gameObject.activeSelf)
                {
                    l.pos = l.tgt.pos;
                }
                _lockOnTgtLog[i] = l;
            }
        }

        public override void CameraReset()
        {
            lockOnTgtGazeCameraPosition = defaultLockOnTgtGazeCameraPosition;
        }
    }
}