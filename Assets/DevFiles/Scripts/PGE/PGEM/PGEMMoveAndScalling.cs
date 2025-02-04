using clrev01.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using static clrev01.Extensions.ExUI;

namespace clrev01.PGE.PGEM
{
    public partial class PGEManager
    {
        public Transform backGround, blocks, scalingTgt;
        public Scrollbar scalingScrollbar;
        public float loopLength = 100;
        private Vector3 _currentMouse, _backGroundStart, _blockStart;
        public float nowScale = 1;
        public float scalingMax = 2, scalingMin = 0.05f;
        public float scalingMagni = 0.5f;

        [SerializeField, ReadOnly]
        float touchStartDist, touchStartScale;
        [SerializeField, ReadOnly]
        bool doTouchScaling = false;


        private void InitScroll()
        {
            scalingScrollbar.onValueChanged.AddListener(f => ScrollBarScaling(f));
            SetScaleScrollbarValue();
            SetConnectLineTextureScale();
        }

        private void ScrollScaling()
        {
            float scrollWheel = Input.GetAxisRaw("Mouse ScrollWheel");
            nowScale += scrollWheel * scalingMagni;
            ScalingExe();
        }

        private void ScalingExe()
        {
            nowScale = Mathf.Clamp(nowScale, scalingMin, scalingMax);
            scalingTgt.localScale = new Vector3(nowScale, nowScale, 1);
            SetScaleScrollbarValue();
            SetConnectLineTextureScale();
        }
        private void SetScaleScrollbarValue()
        {
            scalingScrollbar.SetValueWithoutNotify((nowScale - scalingMin) / (scalingMax - scalingMin));
        }
        private void SetConnectLineTextureScale()
        {
            foreach (var pgb in pgbList)
            {
                if (pgb == null || !pgb.gameObject.activeSelf) continue;
                pgb.ConnectLineTextureScaleUpdate(nowScale);
            }
        }

        void DragMoveStart()
        {
            _currentMouse = GetPointerPos();
            _backGroundStart = backGround.position;
            _blockStart = blocks.position;
        }
        void DragMoveExe()
        {
            TouchScaling();
            if (doTouchScaling) return;
            DragMoveRepeat(backGround, _backGroundStart);
            DragMove(blocks, _blockStart);
        }
        void DragMove(Transform t, Vector3 start)
        {
            t.position = start + (GetPointerPos() - _currentMouse);
        }
        void DragMoveRepeat(Transform t, Vector3 start)
        {
            DragMove(t, start);
            Vector3 v = t.localPosition;
            v.x = Mathf.Repeat(v.x, loopLength);
            v.y = Mathf.Repeat(v.y, loopLength);
            t.localPosition = v;
        }

        private void ScrollBarScaling(float f)
        {
            nowScale = scalingMin + (scalingMax - scalingMin) * f;
            ScalingExe();
        }
        private void TouchScaling()
        {
            if (Input.touchCount >= 2)
            {
                Touch t1 = Input.GetTouch(0);
                Touch t2 = Input.GetTouch(1);
                float pinchDist = Vector2.Distance(t1.position, t2.position);
                if (!doTouchScaling)
                {
                    doTouchScaling = true;
                    touchStartDist = pinchDist;
                    touchStartScale = nowScale;
                }
                else
                {
                    nowScale = touchStartScale * (pinchDist / touchStartDist);
                    ScalingExe();
                }
            }
        }
    }
}