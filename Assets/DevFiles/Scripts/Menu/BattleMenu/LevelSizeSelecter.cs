using clrev01.Bases;
using clrev01.Menu.Dialog;
using TMPro;
using UnityEngine;

namespace clrev01.Menu.BattleMenu
{
    public class LevelSizeSelecter : BaseOfCL
    {
        [SerializeField]
        MenuButton levelSizeButton;
        [SerializeField]
        TextMeshProUGUI levelSizeText;

        [SerializeField]
        QuickMenuDialog quickMenuDialog;


        private void Awake()
        {
            levelSizeButton.OnClick.AddListener(() => OnLevelSizeChange());
        }

        private void OnEnable()
        {
            levelSizeText.text = StaticInfo.Inst.actionLevelHub.levelSizes[StaticInfo.Inst.testMatch.levelSizeNumber].sizeName;
        }

        void OnLevelSizeChange()
        {
            quickMenuDialog.OpenQuickMenu(
                StaticInfo.Inst.actionLevelHub.levelSizes.ConvertAll(x => x.sizeName),
                (int i) =>
                {
                    StaticInfo.Inst.testMatch.levelSizeNumber = i;
                    levelSizeText.text = StaticInfo.Inst.actionLevelHub.levelSizes[StaticInfo.Inst.testMatch.levelSizeNumber].sizeName;
                }
            );
        }
    }
}