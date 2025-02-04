using clrev01.Menu.CycleScroll;
using TMPro;
using UnityEngine;

namespace clrev01.Menu.BattleMenu.Arena
{
    public class ArenaButtonFunc : MenuFunction
    {
        public CycleScrollPanel cycleScrollPanel;
        [SerializeField]
        private TextMeshProUGUI title;
        [SerializeField]
        private TextMeshProUGUI description;

        public void SetIndicate(string titleStr, string descriptionStr)
        {
            title.text = titleStr;
            description.text = descriptionStr;
        }
    }
}