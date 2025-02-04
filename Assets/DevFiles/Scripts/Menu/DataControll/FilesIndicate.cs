using clrev01.Bases;
using clrev01.Menu.CycleScroll;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Menu.DataControll
{
    public class FilesIndicate : BaseOfDataIndicate<FileIndicatePanelFunc>
    {
        public enum FileIndFunction
        {
            NowEdit = -1,
        }

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

        protected override List<string> quickMenuTexts => new(Enum.GetNames(typeof(QuickMenus)));
        protected override List<string> quickMenuTextsOnMulti => new(Enum.GetNames(typeof(QuickMenusOnMulti)));

        protected override void SettingIndStrings()
        {
            base.SettingIndStrings();
            for (int i = 0; i < dataManager.nowSelectableFiles.fileNames.Count; i++)
            {
                IndStrings.Add(dataManager.nowSelectableFiles.fileNames[i]);
                IndFunctions.Add(i);
                IndSelectable.Add(dataManager.selectorMode != DataManager.SelectorMode.Paste);
            }
        }
        protected override List<bool> GetPasteDatas()
        {
            return dataManager.nowSelectableFiles.GetMoveFilesInNowDir();
        }
        protected override void SelectPanel(CycleScrollPanel panel)
        { }
        protected override void SelectDuringSelection(CycleScrollPanel panel)
        {
            FileIndFunction fc = (FileIndFunction)panel.functionCode;

            switch (fc)
            {
                case FileIndFunction.NowEdit:
                    break;
                default:
                    SelectData(panel.functionCode, panel);
                    break;
            }
        }
        protected override void DoubleSelectPanel(CycleScrollPanel panel)
        { }

        //protected override void OnBeginDragPanel(CycleScrollPanel panel)
        //{
        //    if (panel.functionCode < 0) return;
        //    Debug.Log(panel.name);
        //    if (dataManager.selecterMode == DataManager.SelecterMode.multipleChice)
        //    {
        //        dataManager.SettingMoveMulti();
        //    }
        //    else
        //    {
        //        dataManager.nowSelectableFiles.SetMoveFiles(new int[] { panel.functionCode });
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
                    dataManager.DeleteFileStandby(panel.functionCode);
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
            switch (panel.functionCode)
            {
                default:
                    if (currentTxt == input) break;
                    dataManager.RenameFileExecution(panel.functionCode, input).Forget();
                    break;
            }
        }

        protected override void SelectData(int num, CycleScrollPanel panel)
        {
            switch (dataManager.selectorMode)
            {
                case DataManager.SelectorMode.Load:
                    dataManager.LoadExecution(num);
                    MPPM.ReturnPage();
                    break;
                case DataManager.SelectorMode.Save:
                    dataManager.OverwriteFileName(num);
                    break;
                case DataManager.SelectorMode.MultipleChoice:
                    break;
                case DataManager.SelectorMode.Rename:
                    break;
                default:
                    break;
            }
        }
    }
}