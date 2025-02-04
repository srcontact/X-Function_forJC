using clrev01.Bases;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace clrev01.Menu
{
    public class MenuButton :
        Selectable,
        IPointerClickHandler,
        IDropHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        public UnityEvent OnClick = new UnityEvent();
        public UnityEvent OnClickDuringSelection = new UnityEvent();
        public UnityEvent OnRightClick = new UnityEvent();
        public UnityEvent OnDoubleClick = new UnityEvent();
        public Action<PointerEventData> OnDrop;
        public Action<PointerEventData> OnBeginDrag;
        public Action<PointerEventData> OnDrag;
        public Action<PointerEventData> OnEndDrag;
        [SerializeField]
        ColorBlockAsset _colorSetting;
        public ColorBlockAsset colorSetting
        {
            get
            {
                if (_colorSetting != null) return _colorSetting;
                if (StaticInfo.Inst != null) return StaticInfo.Inst.normalColorInfo;
                return ScriptableObject.CreateInstance<ColorBlockAsset>();
            }
            set
            {
                _colorSetting = value;
                SetColors();
            }
        }
        [SerializeField]
        private ColorBlockAsset _toggledColorSetting;
        public ColorBlockAsset toggledColorSetting
        {
            get
            {
                if (_toggledColorSetting != null) return _toggledColorSetting;
                if (StaticInfo.Inst != null) return StaticInfo.Inst.toggledColorInfo;
                return ScriptableObject.CreateInstance<ColorBlockAsset>();
            }
            set
            {
                _toggledColorSetting = value;
                SetColors();
            }
        }

        [SerializeField, ReadOnly]
        int selectCountOnSelection = 0;
        [SerializeField]
        bool _isHighlight;
        public bool isHighlight
        {
            get => _isHighlight;
            set
            {
                _isHighlight = value;
                SetColors();
            }
        }
        public int doubleClickInterval = 20, longPushCount = 45;
        bool isSelected, isLongPushed, isDragged;
        int latestClickFrame;

        protected override void Awake()
        {
            SetColors();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            selectCountOnSelection = 0;
        }

        private bool _isDestroyed;
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _isDestroyed = true;
        }

        private void SetColors()
        {
            if (isHighlight)
            {
                colors = toggledColorSetting.colorBlock;
            }
            else
            {
                colors = colorSetting.colorBlock;
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (!interactable) return;
            isLongPushed = false;
            LongPushCheck().Forget();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable || isLongPushed) return;
            ClickCheck(eventData);
            return;
        }

        private void ClickCheck(PointerEventData eventData)
        {
            int nowF = Time.frameCount;
            switch (eventData.pointerId)
            {
                case int n when n >= -1:
                    OnClick.Invoke();
                    if (isSelected && selectCountOnSelection > 0)
                    {
                        OnClickDuringSelection.Invoke();
                    }
                    selectCountOnSelection++;
                    if (nowF - latestClickFrame < doubleClickInterval)
                    {
                        OnDoubleClick.Invoke();
                    }
                    break;
                case -2:
                    OnRightClick.Invoke();
                    break;
                default:
                    break;
            }
            latestClickFrame = nowF;
        }

        private async UniTask LongPushCheck()
        {
            for (int i = 0; i < longPushCount; i++)
            {
                if (_isDestroyed || gameObject == null) return;
                if (!IsPressed() || isDragged) return;
                await UniTask.Yield();
            }
            isLongPushed = true;
            OnRightClick.Invoke();
        }

        void OnMouseClick(PointerEventData eventData)
        {
            switch (eventData.pointerId)
            {
                case -1:
                    if (eventData.clickCount <= 1) OnClick.Invoke();
                    else if (eventData.clickCount == 2) OnDoubleClick.Invoke();
                    break;
                case -2:
                    OnRightClick.Invoke();
                    break;
                default:
                    break;
            }
        }

        void OnTapClick(PointerEventData eventData)
        {
            if (eventData.pointerId == 0) OnClick.Invoke();
            else if (eventData.pointerId == 1) OnDoubleClick.Invoke();
        }

        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            if (OnDrop == null || !interactable) return;
            OnDrop.Invoke(eventData);
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if (OnBeginDrag == null)
            {
                ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.beginDragHandler);
                return;
            }
            if (!interactable) return;
            isDragged = true;
            OnBeginDrag.Invoke(eventData);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (OnDrag == null)
            {
                ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.dragHandler);
                return;
            }
            if (!interactable) return;
            OnDrag.Invoke(eventData);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (OnEndDrag == null)
            {
                ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.endDragHandler);
                return;
            }
            if (!interactable) return;
            isDragged = false;
            OnEndDrag.Invoke(eventData);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            isSelected = true;
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            isSelected = false;
            selectCountOnSelection = 0;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            SetColors();
        }
#endif
    }
}