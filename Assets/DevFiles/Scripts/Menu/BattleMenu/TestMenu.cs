using clrev01.Bases;
using clrev01.Extensions;
using clrev01.Menu.BattleMenu.Arena;
using clrev01.Menu.TeamEditMenu;
using clrev01.Save;
using UnityEngine;

namespace clrev01.Menu.BattleMenu
{
    public class TestMenu : BaseOfCL, IDataTransport<SaveData>
    {
        [SerializeField]
        MenuButton testExeButton, matchEditButton;
        [SerializeField]
        MatchEditManager matchEditManager;

        public SaveData tData
        {
            get => StaticInfo.Inst.testMatch;
            set => StaticInfo.Inst.testMatch = (MatchData)value;
        }

        private void Awake()
        {
            matchEditButton.OnClick.AddListener(OnMatchEdit);
            testExeButton.OnClick.AddListener(OnTestStart);
        }
        private void OnEnable()
        {
            testExeButton.interactable = tData != null && ((MatchData)tData).startPossible;
        }
        private void OnMatchEdit()
        {
            matchEditManager.isTestDataEdit = true;
            matchEditManager.SetEditTgt(this);
        }
        private void OnTestStart()
        {
            StaticInfo.Inst.PlayMatch = StaticInfo.Inst.testMatch;
            StaticInfo.Inst.battleExecutionData.SetBattleExeData(BattleModeType.Test, BattleRuleType.RemainingHp);
        }
    }
}