using clrev01.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace clrev01.Menu.CycleScroll
{
    /// <summary>
    /// 表示用のパネルをリサイクルして表示を行うスクロール
    /// </summary>
    public class CycleScroll :
        UIBehaviour,
        IScrollHandler,
        IDragHandler,
        IBeginDragHandler
    {
        public Scrollbar scrollbar;
        #region indicateRect
        [SerializeField]
        private RectTransform indicateRect;
        #endregion
        [SerializeField]
        protected CycleScrollPanel originalPanel;
        [SerializeField]
        protected int panelNum;
        [SerializeField]
        protected List<CycleScrollPanel> indicatorList = new();
        [SerializeField]
        private int allIndicateObjNum;
        [SerializeField, Sirenix.OdinInspector.ReadOnly]
        protected int maxScrollableNum, currentScrollValue;
        [SerializeField]
        private CycleScrollEvent onSettingPanel = new();
        public bool isSelectSettable = true;

        private Vector2 _dragBasePos;
        [SerializeField]
        private float scrollSpeedRateOnDrug = 5;

        public enum SelectMode
        {
            Single,
            Multi,
            Disable,
        }

        #region selectMulti
        [SerializeField]
        private SelectMode _selectMulti;
        public SelectMode selectMulti
        {
            get => _selectMulti;
            set
            {
                _selectMulti = value;
                ResetSelects();
            }
        }
        #endregion
        public List<bool> indSelectModes = new();


        protected override void Awake()
        {
            base.Awake();
            RegisterListeners();
        }

        private void InstantiatePanels()
        {
            for (int i = 0; i < panelNum; i++)
            {
                CycleScrollPanel np = originalPanel.SafeInstantiate("_" + i.ToString("000"));
                np.panelId = i;
                np.transform.SetParent(indicateRect.transform);
                np.transform.localScale = Vector3.one;
                np.transform.localPosition = Vector3.zero;
                np.itemId = i;
                np.eventOnSelect.AddListener(OnSelect);
                np.SetOnDrag((PointerEventData d) => OnDrag(d));
                indicatorList.Add(np);
            }
        }

        protected virtual void RegisterListeners()
        {
            scrollbar.onValueChanged.AddListener(OnScrollExe);
        }

        /// <summary>
        /// CycleScrollの初期化処理。
        /// ！CycleScrollを使用するスクリプト側から呼び出す！
        /// </summary>
        /// <param name="settingAction">Panelの表示処理</param>
        /// <returns></returns>
        public List<CycleScrollPanel> Initialize(UnityAction<CycleScrollPanel> settingAction)
        {
            InstantiatePanels();
            onSettingPanel.RemoveListener(settingAction);
            onSettingPanel.AddListener(settingAction);
            return new List<CycleScrollPanel>(indicatorList);
        }

        public virtual void UpdatePage(int max, List<bool> initSelected = null)
        {
            SetMaxIndicateNum(max, initSelected);
            for (var i = 0; i < indicatorList.Count; i++)
            {
                var ii = i + currentScrollValue;
                indicatorList[i].itemId = ii;
                SettingPanel(indicatorList[i]);
                //使い方違う。セッティングの基本はこっちでやる。（今はデータマネージャでやってる）表示内容だけOnSettingPanelで行う。
            }
        }

        private void SetMaxIndicateNum(int max, List<bool> initSelected = null)
        {
            allIndicateObjNum = max;
            maxScrollableNum = allIndicateObjNum - indicatorList.Count;
            indSelectModes.Clear();
            for (int i = 0; i < allIndicateObjNum; i++)
            {
                if (initSelected != null && initSelected.Count > i)
                {
                    indSelectModes.Add(initSelected[i]);
                }
                else indSelectModes.Add(false);
            }
            if (maxScrollableNum < 0) maxScrollableNum = 0;
            currentScrollValue = Math.Min(currentScrollValue, maxScrollableNum);
            ScrollBarLengthSet();
            OnScrollExe(scrollbar.value);
        }

        private void ScrollBarLengthSet()
        {
            if (indicatorList.Count > allIndicateObjNum)
            {
                scrollbar.size = 1;
                scrollbar.gameObject.SetActive(false);
                return;
            }
            scrollbar.numberOfSteps = allIndicateObjNum;
            scrollbar.size = ((float)indicatorList.Count) / ((float)allIndicateObjNum);
            scrollbar.gameObject.SetActive(true);
        }

        protected virtual void OnScrollExe(float v)
        {
            if (v > 1) scrollbar.value = v = 1;
            if (v < 0) scrollbar.value = v = 0;
            if (maxScrollableNum <= 0) return;
            var sv = Mathf.RoundToInt(maxScrollableNum * (1f - v));
            while (sv > currentScrollValue)
            {
                var ind = indicatorList[0];
                indicatorList.RemoveAt(0);
                indicatorList.Add(ind);
                ind.transform.SetAsLastSibling();
                currentScrollValue++;
                ind.itemId = currentScrollValue + indicatorList.Count - 1;
                SettingPanel(ind);
            }
            while (sv < currentScrollValue)
            {
                var end = indicatorList.Count - 1;
                var ind = indicatorList[end];
                indicatorList.RemoveAt(end);
                indicatorList.Insert(0, ind);
                ind.transform.SetAsFirstSibling();
                currentScrollValue--;
                ind.itemId = currentScrollValue;
                SettingPanel(ind);
            }
        }

        public virtual void ResetScrollValue()
        {
            scrollbar.value = 1;
            currentScrollValue = 0;
        }

        public void OnScroll(PointerEventData eventData)
        {
            float sv = 1f / maxScrollableNum;
            ;
            if (eventData.scrollDelta.y < 0) sv = -sv;
            scrollbar.value += sv;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragBasePos = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 v = eventData.position;
            v -= _dragBasePos;
            if (Mathf.Abs(v.y) > indicateRect.rect.height / indicatorList.Count / scrollSpeedRateOnDrug)
            {
                _dragBasePos = eventData.position;
                float sv = 1f / maxScrollableNum;
                ;
                if (v.y > 0) sv = -sv;
                scrollbar.value += sv;
            }
        }

        public void SetSelect(int num)
        {
            if (!isSelectSettable) return;
            switch (selectMulti)
            {
                case SelectMode.Single:
                    for (int i = 0; i < indSelectModes.Count; i++)
                    {
                        indSelectModes[i] = i == num;
                    }
                    break;
                case SelectMode.Multi:
                    indSelectModes[num] = !indSelectModes[num];
                    break;
                default:
                    return;
            }
            if (indSelectModes.Count == 0) return;
            foreach (var indicator in indicatorList)
            {
                indicator.isSelected = indSelectModes.Count > indicator.itemId && indSelectModes[indicator.itemId];
            }
        }
        private void ResetSelects()
        {
            for (int i = 0; i < indSelectModes.Count; i++)
            {
                indSelectModes[i] = false;
            }
            for (int i = 0; i < indicatorList.Count; i++)
            {
                indicatorList[i].isSelected = false;
            }
        }
        private void OnSelect(CycleScrollPanel panel)
        {
            SetSelect(panel.itemId);
        }

        public void SetScrollPosToFirstSelect()
        {
            var firstSelect = indSelectModes.FindIndex(x => x);
            scrollbar.value = 1f - ((float)Mathf.Min(firstSelect, maxScrollableNum) / maxScrollableNum);
        }

        private void SettingPanel(CycleScrollPanel panel)
        {
            var num = panel.itemId;
            if (allIndicateObjNum > num)
            {
                panel.tgtButtonInteractive = true;
                panel.isSelected = indSelectModes[num];
            }
            else
            {
                panel.tgtButtonInteractive = false;
                panel.isSelected = false;
            }
            onSettingPanel.Invoke(panel);
        }
        public int[] GetSelectNums()
        {
            var il = new List<int>();
            for (var i = 0; i < indSelectModes.Count; i++)
            {
                if (indSelectModes[i]) il.Add(i);
            }
            return il.ToArray();
        }

        //todo:サイクルスクロール本体を選択できるようにし、選択中に上下ボタンで選択の上下を行えるようにしたいね。
    }
}