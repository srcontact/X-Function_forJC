using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace clrev01.Menu.CycleScroll
{
    public class CycleScrollPanel : MenuFunction
    {
        [SerializeField]
        ColorBlockAsset selectedColorInfo;
        /// <summary>
        /// パネル自体のID。
        /// スクロールで順序が変わっても不変。
        /// CycleScroll.IndicaterList内での並び順。
        /// </summary>
        public int panelId;
        /// <summary>
        /// 表示しているアイテムのID。
        /// 正確には、表示中のアイテムリスト内で、アイテムが何番目であるかを指す。
        /// </summary>
        public int itemId;
        public int functionCode;
        #region isSelected
        [SerializeField]
        private bool _isSelected;
        public bool isSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnSelectChange();
            }
        }
        public void OnSelectChange()
        {
            if (_isSelected)
            {
                tgtButton.colorSetting = selectedColorInfo;
            }
            else
            {
                tgtButton.colorSetting = null;
            }
        }
        #endregion

        [NonSerialized]
        public CycleScrollEvent eventOnSelect = new CycleScrollEvent();
        [NonSerialized]
        public CycleScrollEvent eventOnSelectDuringSelection = new CycleScrollEvent();
        [NonSerialized]
        public CycleScrollEvent eventOnDoubleClick = new CycleScrollEvent();
        [NonSerialized]
        public CycleScrollEvent eventOnRightSelect = new CycleScrollEvent();

        protected override void Awake()
        {
            base.Awake();
            //DisactiveButtonNavigation();
        }
        public override void ExeOnDoubleClick()
        {
            base.ExeOnDoubleClick();
            eventOnDoubleClick.Invoke(this);
        }
        public override void ExeOnClick()
        {
            base.ExeOnClick();
            eventOnSelect.Invoke(this);
        }
        public override void ExeOnClickDuringSelection()
        {
            base.ExeOnClickDuringSelection();
            eventOnSelectDuringSelection.Invoke(this);
        }
        public override void ExeOnRightClick()
        {
            base.ExeOnRightClick();
            eventOnRightSelect.Invoke(this);
        }

        public void SetOnDrag(Action<PointerEventData> onDrag)
        {
            tgtButton.OnDrag += onDrag;
        }
        private void OnValidate()
        {
            //DisactiveButtonNavigation();
        }

        //private void DisactiveButtonNavigation()
        //{
        //    if (tgtButton != null && tgtButton.navigation.mode != UnityEngine.UI.Navigation.Mode.None)
        //    {
        //        UnityEngine.UI.Navigation nv = tgtButton.navigation;
        //        nv.mode = UnityEngine.UI.Navigation.Mode.None;
        //        tgtButton.navigation = nv;
        //    }
        //}
    }
}