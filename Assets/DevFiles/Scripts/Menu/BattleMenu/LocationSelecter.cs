using clrev01.Bases;
using clrev01.Menu.Dialog;
using TMPro;
using UnityEngine;

namespace clrev01.Menu.BattleMenu
{
    public class LocationSelecter : BaseOfCL
    {
        [SerializeField]
        MenuButton locationButton;
        [SerializeField]
        TextMeshProUGUI locationName;
        [SerializeField]
        QuickMenuDialog quickMenuDialog;


        private void Awake()
        {
            locationButton.OnClick.AddListener(() => OnLocationChange());
        }

        private void OnEnable()
        {
            locationName.text = StaticInfo.Inst.actionLevelHub.levels[StaticInfo.Inst.testMatch.playLevelNum].levelName;
        }

        private void OnLocationChange()
        {
            quickMenuDialog.OpenQuickMenu(
                StaticInfo.Inst.actionLevelHub.levels.ConvertAll(x => x.levelName),
                (int i) =>
                {
                    StaticInfo.Inst.testMatch.playLevelNum = i;
                    locationName.text = StaticInfo.Inst.actionLevelHub.levels[StaticInfo.Inst.testMatch.playLevelNum].levelName;
                }
            );
        }
    }
}