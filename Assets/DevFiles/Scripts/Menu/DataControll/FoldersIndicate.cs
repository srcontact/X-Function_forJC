using clrev01.Menu.CycleScroll;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

namespace clrev01.Menu.DataControll
{
    public class FoldersIndicate : BaseOfDataIndicate<FolderIndicatePanelFunc>
    {
        [SerializeField]
        string
            returnTxt = "Retun",
            presetsTxt = "Presets",
            createNewTxt = "CreateNew",
            newFolderDefaultName = "NewFolder";

        public enum QuickMenus
        {
            Copy,
            Cut,
            Rename,
            Delete,
        }

        public enum QuickMenusOnMulti
        {
            CopyAll,
            CutAll,
            DeleteAll,
        }

        protected override List<string> quickMenuTexts
        {
            get
            {
                return new List<string>(Enum.GetNames(typeof(QuickMenus)));
            }
        }
        protected override List<string> quickMenuTextsOnMulti
        {
            get
            {
                return new List<string>(Enum.GetNames(typeof(QuickMenusOnMulti)));
            }
        }
        [SerializeField]
        public TextMeshProUGUI directryTxt;

        public enum FolderIndFucntion
        {
            returnD = -1,
            create = -2,
            preset = -3,
        }

        //protected override void InitializePanel(List<CycleScrollPanel> panels, int i, FolderIndicatePanelFunc ndp)
        //{
        //    base.InitializePanel(panels, i, ndp);
        //    ndp.onDrop.AddListener(OnDropPanel);
        //}
        protected override void SettingIndStrings()
        {
            base.SettingIndStrings();
            directryTxt.text = Path.DirectorySeparatorChar + dataManager.nowSelectableFiles.nowDirName;
            if (dataManager.isPresetMode) StringsOnPresets();
            else StringsOnFiles();
        }

        private void StringsOnFiles()
        {
            if (dataManager.nowSelectableFiles.isTopDir)
            {
                if (dataManager.selectorMode == DataManager.SelectorMode.Load)
                {
                    if (dataManager.isPresetMode) IndStrings.Add(returnTxt);
                    else IndStrings.Add(presetsTxt);
                    IndFunctions.Add((int)FolderIndFucntion.preset);
                    IndSelectable.Add(true);
                }
            }
            else
            {
                IndStrings.Add(returnTxt);
                IndFunctions.Add((int)FolderIndFucntion.returnD);
                if (dataManager.selectorMode == DataManager.SelectorMode.MultipleChoice)
                {
                    IndSelectable.Add(false);
                }
                else IndSelectable.Add(true);
            }
            for (int i = 0; i < dataManager.nowSelectableFiles.subDirectoryNames.Count; i++)
            {
                IndStrings.Add(dataManager.nowSelectableFiles.subDirectoryNames[i]);
                IndFunctions.Add(i);
                IndSelectable.Add(true);
            }
            switch (dataManager.selectorMode)
            {
                case DataManager.SelectorMode.Load:
                case DataManager.SelectorMode.Save:
                    IndStrings.Add(createNewTxt);
                    IndFunctions.Add((int)FolderIndFucntion.create);
                    IndSelectable.Add(true);
                    break;
                default:
                    break;
            }
        }

        private void StringsOnPresets()
        {
            IndStrings.Add(returnTxt);
            IndFunctions.Add((int)FolderIndFucntion.returnD);
            if (dataManager.selectorMode == DataManager.SelectorMode.MultipleChoice)
            {
                IndSelectable.Add(false);
            }
            else IndSelectable.Add(true);
            for (int i = 0; i < dataManager.nowSelectableFiles.subDirectoryNames.Count; i++)
            {
                IndStrings.Add(dataManager.nowSelectableFiles.subDirectoryNames[i]);
                IndFunctions.Add(i);
                IndSelectable.Add(true);
            }
        }

        protected override List<bool> GetPasteDatas()
        {
            List<bool> bl = dataManager.nowSelectableFiles.GetMoveDirsInNowDir();
            bl.Insert(0, false);
            return bl;
        }

        protected override void SelectPanel(CycleScrollPanel panel)
        {
            //SetSelect(num, panel);
        }
        protected override void SelectDuringSelection(CycleScrollPanel panel)
        {
            FolderIndFucntion fc = (FolderIndFucntion)panel.functionCode;

            switch (fc)
            {
                case FolderIndFucntion.returnD:
                    if (dataManager.isPresetMode && dataManager.nowSelectableFiles.isTopDir) SwitchLoadTgt();
                    else ReturnFolder();
                    break;
                case FolderIndFucntion.create:
                    StartCreateFolder(panel);
                    break;
                case FolderIndFucntion.preset:
                    SwitchLoadTgt();
                    break;
                default:
                    //int m = 0;
                    //for (int i = 0; i < IndFunctions.Count; i++)
                    //{
                    //    if (IndFunctions[i] != 0) m++;
                    //    else break;
                    //}
                    //num -= m;
                    SelectData(panel.functionCode, panel);
                    break;
            }
        }
        protected override void DoubleSelectPanel(CycleScrollPanel panel)
        { }

        //protected override void OnBeginDragPanel(CycleScrollPanel panel)
        //{
        //    if (panel.functionCode < 0) return;
        //    if (dataManager.selecterMode == DataManager.SelecterMode.multipleChice)
        //    {
        //        dataManager.SettingMoveMulti();
        //    }
        //    else
        //    {
        //        dataManager.nowSelectableFiles.SetMoveFolder(new int[] { panel.functionCode });
        //    }
        //}
        protected override void QuickMenuAction(int selectNum)
        {
            //todo:?余裕があったら、選択処理も入れる？
            CycleScrollPanel panel = selected;
            selected = null;

            switch ((QuickMenus)selectNum)
            {
                case QuickMenus.Copy:
                    dataManager.SettingPaste(true);
                    break;
                case QuickMenus.Cut:
                    dataManager.SettingPaste(false);
                    break;
                case QuickMenus.Rename:
                    DataIndicatePanelFunc dataPanel = dataPanels[panel.panelId];
                    dataPanel.OpenInputField(dataPanel.titleTxt.text);
                    break;
                case QuickMenus.Delete:
                    dataManager.DeleteFolderStandby(panel.functionCode);
                    break;
                default:
                    break;
            }
        }
        protected override void QuickMenuOnMultiAction(int selectNum)
        {
            CycleScrollPanel panel = selected;
            selected = null;
            switch ((QuickMenusOnMulti)selectNum)
            {
                case QuickMenusOnMulti.CopyAll:
                    dataManager.SettingPaste(true);
                    break;
                case QuickMenusOnMulti.CutAll:
                    dataManager.SettingPaste(false);
                    break;
                case QuickMenusOnMulti.DeleteAll:
                    dataManager.DeleteMultiStandby();
                    break;
                default:
                    break;
            }
        }
        protected override void InputPanel(string input, string currentTxt, CycleScrollPanel panel)
        {
            FolderIndFucntion fc = (FolderIndFucntion)panel.functionCode;

            switch (fc)
            {
                case FolderIndFucntion.create:
                    CreateFolder(input);
                    break;
                default:
                    if (currentTxt == input) break;
                    dataManager.RenameFolderExecution(panel.functionCode, input).Forget();
                    break;
            }
        }

        protected override void SelectData(int num, CycleScrollPanel panel)
        {
            switch (dataManager.selectorMode)
            {
                case DataManager.SelectorMode.Load:
                case DataManager.SelectorMode.Save:
                case DataManager.SelectorMode.Paste:
                    //nowPathLink.selectedSubPathLink = num;
                    dataManager.nowSelectableFiles.LoadFolder(num);
                    dataManager.UpdatePage();
                    break;
                case DataManager.SelectorMode.MultipleChoice:
                    break;
                case DataManager.SelectorMode.Rename:
                    break;
                default:
                    break;
            }
        }

        private void ReturnFolder()
        {
            //dataManager.nowSelectableFiles.pathLink.ReturnFolder();
            dataManager.nowSelectableFiles.LoadFolder(-1);
            dataManager.UpdatePage();
        }

        private void SwitchLoadTgt()
        {
            dataManager.DataTgtSetting(!dataManager.isPresetMode);
            dataManager.UpdatePage();
        }

        void StartCreateFolder(CycleScrollPanel panel)
        {
            //panel.OpenInputField(newFolderDefaultName);
            dataPanels[panel.panelId].OpenInputField(newFolderDefaultName);
        }

        //void OnDropPanel(CycleScrollPanel panel)
        //{
        //    dataManager.MoveExecution(panel.functionCode);
        //    //if (dataManager.selecterMode == DataManager.SelecterMode.multipleChice)
        //    //{
        //    //    //dataManager.MoveMultiStandby(IndStrings[panel.functionCode]);
        //    //    //dataManager.nowSelectableFiles.ExecuteMoveFiles(panel.functionCode);
        //    //}
        //    //if (dataManager.draggedPanel is FolderIndicatePanelFunc)
        //    //{
        //    //    dataManager.nowSelectableFiles.ExecuteMoveFiles(panel.functionCode);
        //    //    //StringBuilder sb = new StringBuilder();
        //    //    //dataManager.nowSelectableFiles.pathLink.GetFolderPath(ref sb);
        //    //    //sb.Append(nowPathLink.subPathLinks[panel.functionCode].directoryName);
        //    //    //dataManager.MoveStandby(sb.ToString(), 
        //    //    //    new int[] { dataManager.draggedPanel.panel.functionCode },
        //    //    //    null
        //    //    //    );
        //    //}
        //    //else
        //    //{

        //    //}
        //}

        void CreateFolder(string input)
        {
            dataManager.CreateFolderExecution(input);
        }
    }
}