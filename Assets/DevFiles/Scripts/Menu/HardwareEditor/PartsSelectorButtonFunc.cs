using clrev01.Menu.CycleScroll;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace clrev01.Menu.HardwareEditor
{
    public class PartsSelectorButtonFunc : MenuFunction
    {
        public CycleScrollPanel cycleScrollPanel;
        public ColorBlockAsset selectedColor;
        [SerializeField]
        TextMeshProUGUI text;
        [SerializeField]
        Image iconImage;

        public void SetIndicate(string str, Sprite icon)
        {
            text.text = str;
            if (icon == null) iconImage.gameObject.SetActive(false);
            else
            {
                iconImage.gameObject.SetActive(true);
                iconImage.sprite = icon;
            }
        }
    }
}