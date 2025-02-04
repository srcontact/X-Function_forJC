using clrev01.Bases;
using clrev01.Menu.CycleScroll;
using clrev01.Menu.Dialog;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace clrev01.Menu.DataControll
{
    public abstract class BaseOfDataIndicate<F> :
        BaseOfCL
        where F : DataIndicatePanelFunc
    {
        [SerializeField]
        protected DataManager dataManager;
        [SerializeField, ReadOnly]
        CycleScroll.CycleScroll _cycleScroll;
        [SerializeField]
        protected QuickMenuDialog quickMenuDialog;
        //protected PathLink nowPathLink
        //{
        //    get
        //    {
        //        return dataManager.nowSelectableFiles.pathLink.GetNowPathLink();
        //    }
        //}

        public List<string> IndStrings = new List<string>();
        public List<int> IndFunctions = new List<int>();
        public List<bool> IndSelectable = new List<bool>();

        public List<F> dataPanels = new List<F>();
        protected CycleScrollPanel selected;
        protected virtual List<string> quickMenuTexts { get; }
        protected virtual List<string> quickMenuTextsOnMulti { get; }

        public CycleScroll.CycleScroll cycleScroll
        {
            get
            {
                if (_cycleScroll == null)
                {
                    _cycleScroll = GetComponent<CycleScroll.CycleScroll>();
                    //_cycleScroll.Initialize(SettingPanel, SelectPanel, DoubleSelectPanel, RightSelectPanel, InputPanel);
                    List<CycleScrollPanel> panels =
                        _cycleScroll.Initialize((CycleScrollPanel cp) => SettingPanel(cp));
                    InitializePanels(panels);
                }
                return _cycleScroll;
            }
        }

        private void InitializePanels(List<CycleScrollPanel> panels)
        {
            for (int i = 0; i < panels.Count; i++)
            {
                F ndp = panels[i].GetComponent<F>();
                dataPanels.Add(ndp);
                InitializePanel(panels, i, ndp);
            }
        }

        protected virtual void InitializePanel(List<CycleScrollPanel> panels, int i, F ndp)
        {
            ndp.onInput.AddListener(InputPanel);
            //ndp.onBeginDrag.AddListener(OnBeginDragPanel);
            //ndp.onEndDrag.AddListener(OnEndDragPanel);

            panels[i].eventOnSelect.AddListener(SelectPanel);
            panels[i].eventOnSelectDuringSelection.AddListener(SelectDuringSelection);
            panels[i].eventOnDoubleClick.AddListener(DoubleSelectPanel);
            panels[i].eventOnRightSelect.AddListener(RightSelectPanel);
        }

        internal virtual void UpdateInd(bool resetScroll)
        {
            SettingIndStrings();
            List<bool> bl = null;
            if (dataManager.selectorMode == DataManager.SelectorMode.Paste)
            {
                bl = GetPasteDatas();
            }
            cycleScroll.UpdatePage(IndStrings.Count, bl);
            if (resetScroll) cycleScroll.ResetScrollValue();
        }

        protected abstract List<bool> GetPasteDatas();

        protected virtual void SettingIndStrings()
        {
            IndStrings.Clear();
            IndFunctions.Clear();
            IndSelectable.Clear();
            cycleScroll.isSelectSettable = dataManager.selectorMode != DataManager.SelectorMode.Paste;
        }

        protected abstract void SelectPanel(CycleScrollPanel panel);

        protected abstract void SelectData(int num, CycleScrollPanel panel);

        protected abstract void SelectDuringSelection(CycleScrollPanel panel);

        private void SettingPanel(CycleScrollPanel panel)
        {
            if (!panel.tgtButtonInteractive)
            {
                dataPanels[panel.panelId].titleTxt.text = string.Empty;
                panel.functionCode = 0;
                return;
            }
            dataPanels[panel.panelId].titleTxt.text = IndStrings[panel.itemId];
            panel.functionCode = IndFunctions[panel.itemId];
            panel.tgtButtonInteractive = IndSelectable[panel.itemId];
        }

        protected abstract void DoubleSelectPanel(CycleScrollPanel panel);
        protected virtual void RightSelectPanel(CycleScrollPanel panel)
        {
            if (panel.functionCode < 0 || dataManager.isPresetMode) return;
            selected = panel;
            if (dataManager.selectorMode == DataManager.SelectorMode.MultipleChoice)
            {
                if (!cycleScroll.indSelectModes[panel.itemId])
                {
                    cycleScroll.SetSelect(panel.itemId);
                }
                quickMenuDialog.OpenQuickMenu(new List<string>(quickMenuTextsOnMulti), QuickMenuOnMultiAction);
            }
            else
            {
                cycleScroll.SetSelect(panel.itemId);
                quickMenuDialog.OpenQuickMenu(new List<string>(quickMenuTexts), QuickMenuAction);
            }
        }
        protected abstract void InputPanel(string input, string currentTxt, CycleScrollPanel panel);

        protected abstract void QuickMenuAction(int selectNum);
        protected abstract void QuickMenuOnMultiAction(int selectNum);

        //protected abstract void OnBeginDragPanel(CycleScrollPanel panel);
        //protected virtual void OnEndDragPanel(CycleScrollPanel panel)
        //{
        //    dataManager.nowSelectableFiles.ResetMoveLists();
        //}

        public int[] GetSelectedNum()
        {
            int[] snl = cycleScroll.GetSelectNums();
            int[] il = new int[snl.Length];
            for (int i = 0; i < snl.Length; i++)
            {
                il[i] = IndFunctions[snl[i]];
            }
            return il;
        }
    }
}