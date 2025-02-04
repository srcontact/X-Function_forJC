using System;
using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.Extensions;
using clrev01.Menu.DataControll;
using clrev01.Menu.EditMenu;
using clrev01.Save;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Menu.TeamEditMenu
{
    public class TeamEditManager : BaseOfCL, IDataTransport<SaveData>
    {
        [SerializeField]
        DataManager dataManager;

        public IDataTransport<SaveData> editTgtData;


        int tDataNum;

        private TeamData editTgtTeam => (TeamData)editTgtData.tData;

        public SaveData tData
        {
            get => editTgtTeam.machineList[tDataNum];
            set => editTgtTeam.machineList[tDataNum] = (CustomData)value;
        }

        [SerializeField]
        MenuPage machineLoader;

        public List<MachineInformation> machineInfos = new List<MachineInformation>();

        [SerializeField]
        private MenuButton resetButton, loadButton, loadPresetButton, saveButton;

        [SerializeField]
        private TMP_InputField nameInputField;

        public bool isTestDataEdit;

        private void Awake()
        {
            resetButton.OnClick.AddListener(() => ResetExe().Forget());
            loadButton.OnClick.AddListener(() => LoadExe());
            loadPresetButton.OnClick.AddListener(() => LoadPresetExe());
            saveButton.OnClick.AddListener(() => SaveExe());
            nameInputField.onEndEdit.AddListener(str => NameInputExe(str));
            for (int i = 0; i < machineInfos.Count; i++)
            {
                int ii = i;
                machineInfos[i].button.OnClickDuringSelection.AddListener(() => OpenMachinePanel(ii));
                machineInfos[i].deleteButton.OnClick.AddListener(() => RemoveMachine(ii));
            }
        }

        private void OnEnable()
        {
            UpdatePage();
        }

        private void UpdatePage()
        {
            editTgtData.tData ??= new TeamData();
            for (int i = 0; i < machineInfos.Count; i++)
            {
                machineInfos[i].numberText.text = (i + 1).ToString("00");
                if (editTgtTeam.machineList.Count <= i) editTgtTeam.machineList.Add(null);
                machineInfos[i].indicateData = editTgtTeam.machineList[i];
            }

            nameInputField.interactable = !isTestDataEdit;
            nameInputField.text = editTgtTeam.dataName;
        }

        private void OpenMachinePanel(int ii)
        {
            tDataNum = ii;
            dataManager.SetLoadTgt(this, DataManager.DataMode.Custom, false, true);
            MenuPagePanelManager.Inst.OpenPage(machineLoader);
        }

        void RemoveMachine(int ii)
        {
            editTgtTeam.machineList[ii] = null;
            UpdatePage();
        }

        public void SetEditTgt(IDataTransport<SaveData> data)
        {
            editTgtData = data;
        }

        private async UniTask ResetExe()
        {
            const string warningTxt = "チームをリセットします。\n現在編集中のチーム内容は削除されます。\nよろしいですか？";
            MPPM.dialogManager.simpleDialog.OpenSimpleDialogCloseOnlyButton(
                warningTxt,
                new[] { "Yes", "No" },
                new[]
                {
                    (Action)(() =>
                    {
                        editTgtData.tData = new TeamData();
                        UpdatePage();
                    })
                }
            );
        }

        private void LoadExe()
        {
            dataManager.SetLoadTgt(editTgtData, DataManager.DataMode.Team, false, false);
        }

        private void LoadPresetExe()
        {
            dataManager.SetLoadTgt(editTgtData, DataManager.DataMode.Team, true, false);
        }

        private void SaveExe()
        {
            dataManager.SetSaveTgt(editTgtData, DataManager.DataMode.Team);
        }

        private void NameInputExe(string arg0)
        {
            editTgtData.tData.dataName = arg0;
        }
    }
}