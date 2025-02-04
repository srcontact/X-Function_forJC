using clrev01.Bases;
using clrev01.Extensions;
using clrev01.Save;
using clrev01.Save.DataManageObj;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Menu.DataControll
{
    public class DataManager : BaseOfCL
    {
        public enum DataMode
        {
            Custom,
            Team,
            Match,
            Replay,
            PlayData,
        }

        public enum SelectorMode
        {
            Load,
            Save,
            MultipleChoice,
            Rename,
            Paste,
        }

        #region selecterMode

        [SerializeField]
        private SelectorMode _selectorMode;
        [SerializeField, ReadOnly]
        private SelectorMode currentSelectorMode;
        public SelectorMode selectorMode
        {
            get => _selectorMode;
            private set
            {
                if (_selectorMode is SelectorMode.Load or SelectorMode.Save)
                {
                    currentSelectorMode = _selectorMode;
                }
                _selectorMode = value;
                switch (_selectorMode)
                {
                    case SelectorMode.Save:
                    case SelectorMode.Rename:
                        inputZone.SetActive(true);
                        if (nowSelectableFiles.isPreset)
                        {
                            throw new Exception("Presets cannot be renamed.");
                        }
                        break;
                    default:
                        inputZone.SetActive(false);
                        break;
                }
                ButtonsActivate();
                if (_selectorMode == SelectorMode.MultipleChoice)
                {
                    foldersIndicate.cycleScroll.selectMulti = CycleScroll.CycleScroll.SelectMode.Multi;
                    filesIndicate.cycleScroll.selectMulti = CycleScroll.CycleScroll.SelectMode.Multi;
                    switchMultipleChoiceButton.colorSetting = switchMultipleChoiceButtonColorInfo;
                }
                else
                {
                    foldersIndicate.cycleScroll.selectMulti = CycleScroll.CycleScroll.SelectMode.Single;
                    filesIndicate.cycleScroll.selectMulti = CycleScroll.CycleScroll.SelectMode.Single;
                    switchMultipleChoiceButton.colorSetting = null;
                }
            }
        }

        #endregion

        private IDataTransport<SaveData> _dataTgt;

        #region nowCDManageObj

        private IDataManageObj _nowSelectableFiles;
        public IDataManageObj nowSelectableFiles
        {
            get => _nowSelectableFiles;
            set
            {
                _nowSelectableFiles = value;
                ButtonsActivate();
            }
        }

        #endregion

        #region CustomData

        [SerializeField]
        private string _rootPathOfCD = $"{Path.DirectorySeparatorChar}SaveData{Path.DirectorySeparatorChar}CustomData";
        public string rootPathOfCD
        {
            get
            {
#if UNITY_EDITOR
                return Application.dataPath + _rootPathOfCD;
#else
            return Application.persistentDataPath + _rootPathOfCD;
#endif
            }
        }

        private FileManageObjCD _fileManagerCD;
        public FileManageObjCD fileManagerCD => _fileManagerCD ??= new FileManageObjCD(rootPathOfCD);

        public PresetManageAssetCD presetManageAssetCD;
        public PresetManageObjCD presetManagerCD => presetManageAssetCD.asset;

        #endregion

        #region TeamData

        [SerializeField]
        private string _rootPathOfTD = $"{Path.DirectorySeparatorChar}SaveData{Path.DirectorySeparatorChar}TeamData";
        public string rootPathOfTD
        {
            get
            {
#if UNITY_EDITOR
                return Application.dataPath + _rootPathOfTD;
#else
            return Application.persistentDataPath + _rootPathOfTD;
#endif
            }
        }

        private FileManageObjTD _fileManagerTD;
        public FileManageObjTD fileManagerTD => _fileManagerTD ??= new FileManageObjTD(rootPathOfTD);

        public PresetManageAssetTD presetManageAssetTD;
        public PresetManageObjTD presetManagerTD => presetManageAssetTD.asset;

        #endregion

        #region MatchData

        [SerializeField]
        private string _rootPathOfMD = $"{Path.DirectorySeparatorChar}SaveData{Path.DirectorySeparatorChar}MatchData";
        public string rootPathOfMD
        {
            get
            {
#if UNITY_EDITOR
                return Application.dataPath + _rootPathOfMD;
#else
            return Application.persistenMDataPath + _rootPathOfMD;
#endif
            }
        }

        private FileManageObjMD _fileManagerMD;
        public FileManageObjMD fileManagerMD => _fileManagerMD ??= new FileManageObjMD(rootPathOfMD);

        public PresetManageAssetMD presetManageAssetMD;
        public PresetManageObjMD presetManagerMD => presetManageAssetMD.asset;

        #endregion

        public FoldersIndicate foldersIndicate;
        public FilesIndicate filesIndicate;
        public GameObject inputZone;
        public TMP_InputField inputField;
        public MenuButton nowEditDataButton;
        private bool indicateNowEditData;
        private DataMode dataMode;
        public MenuButton
            saveButton,
            switchMultipleChoiceButton,
            deleteButton,
            copyButton,
            cutButton,
            pasteButton,
            cancelButton;
        [SerializeField]
        private ColorBlockAsset switchMultipleChoiceButtonColorInfo;
        public bool isPresetMode => nowSelectableFiles.isPreset;
        private bool _cutOrCopy;

        private SaveData _editTgtData;

        private void Awake()
        {
            presetManagerCD.InitializeOnUse();
            presetManagerTD.InitializeOnUse();
            inputField.onEndEdit.AddListener(UpdateFileName);
            saveButton.OnClick.AddListener(() => SaveExecution().Forget());
            switchMultipleChoiceButton.OnClick.AddListener(() => SwitchMultipleChoiceMode());
            deleteButton.OnClick.AddListener(() => DeleteMultiStandby());
            copyButton.OnClick.AddListener(() => SettingPaste(true));
            cutButton.OnClick.AddListener(() => SettingPaste(false));
            pasteButton.OnClick.AddListener(() => MoveExecution(-100).Forget());
            cancelButton.OnClick.AddListener(() => PasteCancel());
            nowEditDataButton.OnClick.AddListener(() => LoadNowEditData());
        }
        private void OnEnable()
        {
            UpdatePage();
        }

        private void ButtonsActivate()
        {
            saveButton.gameObject.SetActive(false);
            switchMultipleChoiceButton.gameObject.SetActive(false);
            deleteButton.gameObject.SetActive(false);
            copyButton.gameObject.SetActive(false);
            cutButton.gameObject.SetActive(false);
            pasteButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
            if (isPresetMode) return;
            switch (_selectorMode)
            {
                case SelectorMode.Load:
                    switchMultipleChoiceButton.gameObject.SetActive(true);
                    break;
                case SelectorMode.Save:
                    saveButton.gameObject.SetActive(true);
                    switchMultipleChoiceButton.gameObject.SetActive(true);
                    break;
                case SelectorMode.MultipleChoice:
                    deleteButton.gameObject.SetActive(true);
                    copyButton.gameObject.SetActive(true);
                    cutButton.gameObject.SetActive(true);
                    switchMultipleChoiceButton.gameObject.SetActive(true);
                    break;
                case SelectorMode.Paste:
                    pasteButton.gameObject.SetActive(true);
                    cancelButton.gameObject.SetActive(true);
                    break;
                case SelectorMode.Rename:
                default:
                    break;
            }
        }
        public void SetLoadTgt(IDataTransport<SaveData> tgt, DataMode dataMode, bool isPreset, bool indicateNowEditData)
        {
            this.dataMode = dataMode;
            DataTgtSetting(isPreset);
            _dataTgt = tgt;
            selectorMode = SelectorMode.Load;
            this.indicateNowEditData = indicateNowEditData;
        }
        public void SetSaveTgt(IDataTransport<SaveData> tgt, DataMode dataMode)
        {
            this.dataMode = dataMode;
            DataTgtSetting(false);
            _dataTgt = tgt;
            _editTgtData = _dataTgt.tData.CloneDeep();
            selectorMode = SelectorMode.Save;
        }
        public void DataTgtSetting(bool isPreset)
        {
            //todo:残りのデータのモードも実装する。
            switch (dataMode)
            {
                case DataMode.Custom:
                    nowSelectableFiles = !isPreset ? fileManagerCD : presetManagerCD;
                    break;
                case DataMode.Team:
                    nowSelectableFiles = !isPreset ? fileManagerTD : presetManagerTD;
                    break;
                case DataMode.Match:
                    nowSelectableFiles = !isPreset ? fileManagerMD : presetManagerMD;
                    break;
                case DataMode.Replay:
                    break;
                case DataMode.PlayData:
                    break;
            }
        }

        public void UpdatePage(bool resetScroll = true)
        {
            nowSelectableFiles.ReloadDirectory();
            foldersIndicate.UpdateInd(resetScroll);
            filesIndicate.UpdateInd(resetScroll);

            if (selectorMode == SelectorMode.Save)
            {
                inputField.text = _editTgtData.fileName;
            }
            nowEditDataButton.gameObject.SetActive(
                selectorMode == SelectorMode.Load &&
                indicateNowEditData &&
                StaticInfo.Inst.nowEditMech != null);
        }
        void UpdateFileName(string newName)
        {
            _editTgtData.fileName = newName;
        }
        public void OverwriteFileName(int dataNum)
        {
            string s = nowSelectableFiles.fileNames[dataNum];
            _editTgtData.fileName = s;
            inputField.text = _editTgtData.fileName;
        }
        private async UniTask SaveExecution()
        {
            void Save()
            {
                var err = nowSelectableFiles.SaveFile(_editTgtData);
                if (err != null)
                {
                    MPPM.dialogManager.simpleDialog.OpenSimpleDialogCloseOnTouchBack(err, new[] { "OK" });
                }
                else
                {
                    _dataTgt.tData = _editTgtData;
                    MPPM.ReturnPage();
                }
            }

            var path = nowSelectableFiles.nowDirName + Path.DirectorySeparatorChar + _editTgtData.fileName + nowSelectableFiles.fileExt;
            if (OverlapCheck(out var dialogTxt, path))
            {
                MPPM.dialogManager.simpleDialog.OpenSimpleDialogCloseOnlyButton(
                    dialogTxt,
                    new[] { "Overwrite", "Cancel" },
                    new[]
                    {
                        (Action)(() => Save())
                    }
                );
            }
            else Save();
        }


        void SwitchMultipleChoiceMode()
        {
            if (selectorMode != SelectorMode.MultipleChoice)
            {
                selectorMode = SelectorMode.MultipleChoice;
            }
            else
            {
                selectorMode = currentSelectorMode;
            }
            UpdatePage(false);
        }

        public void LoadExecution(int dataNum)
        {
            string err = null;
            _dataTgt.tData = nowSelectableFiles.LoadFile(dataNum, ref err).CloneDeep();
            if (err != null)
            {
                MPPM.dialogManager.simpleDialog.OpenSimpleDialogCloseOnTouchBack(err, new[] { "OK" });
            }
        }

        public void DeleteMultiStandby()
        {
            MPPM.dialogManager.simpleDialog.OpenSimpleDialogCloseOnlyButton(
                GetTextOnDelete(
                    foldersIndicate.GetSelectedNum(),
                    filesIndicate.GetSelectedNum()
                ),
                new[] { "OK", "Cancel" },
                new[]
                {
                    (Action)(() => DeleteExecution(foldersIndicate.GetSelectedNum(), filesIndicate.GetSelectedNum()).Forget())
                }
            );
        }
        public void DeleteFileStandby(params int[] tgt)
        {
            MPPM.dialogManager.simpleDialog.OpenSimpleDialogCloseOnlyButton(
                GetTextOnDelete(null, tgt),
                new[] { "OK", "Cancel" },
                new[]
                {
                    (Action)(() => DeleteExecution(null, tgt).Forget())
                }
            );
        }
        public void DeleteFolderStandby(params int[] tgt)
        {
            MPPM.dialogManager.simpleDialog.OpenSimpleDialogCloseOnlyButton(
                GetTextOnDelete(tgt, null),
                new[] { "OK", "Cancel" },
                new[]
                {
                    (Action)(() => DeleteExecution(tgt, null).Forget())
                }
            );
        }
        private async UniTask DeleteExecution(int[] folders, int[] files)
        {
            string err1, err2;
            err1 = err2 = null;
            if (folders != null) err1 = nowSelectableFiles.DeleteFolder(folders);
            if (files != null) err2 = nowSelectableFiles.DeleteFiles(files);

            if (selectorMode == SelectorMode.MultipleChoice)
            {
                selectorMode = currentSelectorMode;
            }

            UpdatePage(false);
            if (err1 != null)
            {
                MPPM.dialogManager.simpleDialog.OpenSimpleDialogCloseOnTouchBack(
                    err1, new[] { "OK" }
                );
            }
            if (err2 != null)
            {
                MPPM.dialogManager.simpleDialog.OpenSimpleDialogCloseOnTouchBack(
                    err2, new[] { "OK" }
                );
            }
        }

        private string GetTextOnDelete(int[] folders, int[] files)
        {
            var t = "";
            var names = new List<string>();
            if (folders is { Length: > 0 })
            {
                foreach (var f in folders)
                {
                    names.Add(nowSelectableFiles.subDirectoryNames[f]);
                }
                t += "Delete " + names.Count + " Folders:\n";
                foreach (var n in names)
                {
                    t += "    " + n + "\n";
                }
            }
            names.Clear();
            if (files is { Length: > 0 })
            {
                foreach (var f in files)
                {
                    names.Add(nowSelectableFiles.fileNames[f]);
                }
                t += "Delete " + names.Count + " Files:\n";
                foreach (var n in names)
                {
                    t += "    " + n + "\n";
                }
            }
            return t;
        }

        public void SettingMoveMulti()
        {
            nowSelectableFiles.SetMoveFolder(foldersIndicate.GetSelectedNum());
            nowSelectableFiles.SetMoveFiles(filesIndicate.GetSelectedNum());
        }
        private async UniTask MoveExecution(int toDirNum)
        {
            void Move()
            {
                var errs = new List<string>();
                errs.AddRange(nowSelectableFiles.ExecuteMoveFolder(toDirNum, _cutOrCopy));
                errs.AddRange(nowSelectableFiles.ExecuteMoveFiles(toDirNum, _cutOrCopy));
                foreach (var err in errs)
                {
                    if (err == null) continue;
                    MPPM.dialogManager.simpleDialog.OpenSimpleDialogCloseOnTouchBack(
                        err, new[] { "OK" }
                    );
                }
                selectorMode = currentSelectorMode;
                UpdatePage(false);
            }

            var paths = nowSelectableFiles.GetMoveFileNames(nowSelectableFiles.nowDirName);
            if (OverlapCheck(out var dialogTxt, paths.ToArray()))
            {
                MPPM.dialogManager.simpleDialog.OpenSimpleDialogCloseOnlyButton(
                    dialogTxt,
                    new[] { "Overwrite", "Cancel" },
                    new[]
                    {
                        (Action)(() => { Move(); })
                    }
                );
            }
            else Move();
        }

        public void SettingPaste(bool cutOrCopy)
        {
            SettingMoveMulti();
            selectorMode = SelectorMode.Paste;
            _cutOrCopy = cutOrCopy;
            UpdatePage(false);
        }

        void PasteCancel()
        {
            selectorMode = currentSelectorMode;
            nowSelectableFiles.ResetMoveLists();
            UpdatePage(false);
        }

        public void CreateFolderExecution(string input)
        {
            string err = nowSelectableFiles.CreateFolder(input);
            if (err != null)
            {
                MPPM.dialogManager.simpleDialog.OpenSimpleDialogCloseOnTouchBack(
                    err,
                    new[] { "OK" }
                );
            }
            UpdatePage(false);
        }

        public async UniTask RenameFileExecution(int functionCode, string input)
        {
            void Rename()
            {
                var errs = nowSelectableFiles.RenameFile(functionCode, input);
                foreach (var err in errs)
                {
                    if (err == null) continue;
                    MPPM.dialogManager.simpleDialog.OpenSimpleDialogCloseOnTouchBack(
                        err,
                        new[] { "OK" }
                    );
                }
                UpdatePage(false);
            }

            var path = nowSelectableFiles.nowDirName + Path.DirectorySeparatorChar + input + nowSelectableFiles.fileExt;
            if (OverlapCheck(out var dialogTxt, path))
            {
                MPPM.dialogManager.simpleDialog.OpenSimpleDialogCloseOnlyButton(
                    dialogTxt,
                    new[] { "Overwrite", "Cancel" },
                    new[]
                    {
                        (Action)(() => { Rename(); })
                    }
                );
            }
            else Rename();
        }
        public async UniTask RenameFolderExecution(int num, string input)
        {
            var errs = nowSelectableFiles.RenameFolder(num, input);
            foreach (var err in errs)
            {
                if (err == null) continue;
                MPPM.dialogManager.simpleDialog.OpenSimpleDialogCloseOnTouchBack(
                    err,
                    new[] { "OK" }
                );
            }
            UpdatePage();
        }


        bool OverlapCheck(out string dialogTxt, params string[] paths)
        {
            var overlaps = new List<string>();

            foreach (var path in paths)
            {
                if (File.Exists(path)) overlaps.Add(path);
            }
            if (overlaps.Count <= 0) dialogTxt = null;
            else
            {
                dialogTxt = "Overlap Files :";
                foreach (var overlap in overlaps)
                {
                    dialogTxt += "\n    " +
                                 overlap.Remove(
                                     0,
                                     nowSelectableFiles.nowDirName.Length + 1).Replace(nowSelectableFiles.fileExt, "");
                }
            }
            return dialogTxt != null;
        }

        private void LoadNowEditData()
        {
            switch (dataMode)
            {
                case DataMode.Custom:
                    _dataTgt.tData = StaticInfo.Inst.nowEditMech;
                    break;
                default:
                    break;
            }
            MPPM.ReturnPage();
        }
    }
}