using clrev01.Bases;
using TMPro;
using UnityEngine;

namespace clrev01.Menu
{
    public class TabMenuTabButton : BaseOfCL
    {
        public MenuButton button;
        public RectTransform rectTransform;
        public TextMeshProUGUI text;
        public int tabNumber { get; set; }
    }
}