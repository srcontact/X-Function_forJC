using System;
using clrev01.Bases;
using clrev01.Extensions;
using clrev01.Save;
using clrev01.Menu.DataControll;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Menu.TeamEditMenu
{
    public class MatchEditManager : BaseOfCL, IDataTransport<SaveData>
    {
        [SerializeField]
        private DataManager dataManager;
        [SerializeField]
        private List<TeamInformation> teamPanels = new List<TeamInformation>();
        [SerializeField]
        private MenuButton resetTeamButton;
        [SerializeField]
        private TeamEditManager teamEditManager;
        [SerializeField]
        private MenuPage teamEditManagerObj;
        [SerializeField]
        private MenuButton loadButton, saveButton;
        public IDataTransport<SaveData> editTgtData;
        private MatchData editTgtMatch => (MatchData)editTgtData.tData;
        private int _editTgtTeamNum;
        public SaveData tData
        {
            get => editTgtMatch.teamList[_editTgtTeamNum];
            set
            {
                editTgtMatch.teamList[_editTgtTeamNum] = (TeamData)value;
                if (isTestDataEdit) editTgtMatch.teamList[_editTgtTeamNum].dataName = "TestTeam_" + _editTgtTeamNum.ToString("00");
            }
        }
        public bool isTestDataEdit;

        private void Awake()
        {
            resetTeamButton.OnClick.AddListener(() => ResetExe().Forget());
            loadButton.OnClick.AddListener(() => LoadExe());
            saveButton.OnClick.AddListener(() => SaveExe());
            for (int i = 0; i < teamPanels.Count; i++)
            {
                int ii = i;
                teamPanels[i].button.OnClickDuringSelection.AddListener(() => OpenTeamPanel(ii));
                teamPanels[i].deleteButton.OnClick.AddListener(() => RemoveTeam(ii));
            }
        }
        private void OnEnable()
        {
            UpdatePage();
        }

        private void UpdatePage()
        {
            if (editTgtMatch == null)
            {
                editTgtData.tData = new MatchData();
            }
            for (int i = 0; i < teamPanels.Count; i++)
            {
                if (editTgtMatch.teamList.Count <= i) editTgtMatch.teamList.Add(null);
                teamPanels[i].indicateData = editTgtMatch.teamList[i];
            }
        }

        private void OpenTeamPanel(int ii)
        {
            _editTgtTeamNum = ii;
            teamEditManager.isTestDataEdit = isTestDataEdit;
            teamEditManager.SetEditTgt(this);
            MenuPagePanelManager.Inst.OpenPage(teamEditManagerObj);
        }
        void RemoveTeam(int ii)
        {
            editTgtMatch.teamList[ii] = null;
            UpdatePage();
        }

        private async UniTask ResetExe()
        {
            var warningTxt = "マッチを新規作成します。\n現在編集中のマッチは削除されます。\nよろしいですか？";
            MPPM.dialogManager.simpleDialog.OpenSimpleDialogCloseOnlyButton(
                warningTxt,
                new[] { "Yes", "No" },
                new[]
                {
                    (Action)(() =>
                    {
                        for (int i = 0; i < editTgtMatch.teamList.Count; i++)
                        {
                            editTgtMatch.teamList[i] = null;
                        }
                        UpdatePage();
                    })
                }
            );
        }

        public void SetEditTgt(IDataTransport<SaveData> data)
        {
            editTgtData = data;
        }

        private void LoadExe()
        {
            dataManager.SetLoadTgt(editTgtData, DataManager.DataMode.Match, false, false);
        }

        private void SaveExe()
        {
            dataManager.SetSaveTgt(editTgtData, DataManager.DataMode.Match);
        }
    }
}