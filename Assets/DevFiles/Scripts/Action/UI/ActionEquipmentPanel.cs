using clrev01.Bases;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace clrev01.ClAction.UI
{
    public class ActionEquipmentPanel : BaseOfCL
    {
        public Image iconImage;
        public TextMeshProUGUI remainingNumText;


        public void SetIndicate(bool ind, Sprite icon, string remainingNum)
        {
            if (iconImage.sprite != icon)
            {
                iconImage.gameObject.SetActive(ind);
                iconImage.sprite = icon;
                remainingNumText.gameObject.SetActive(ind);
            }
            if (!ind) return;
            remainingNumText.text = remainingNum.ToString();
        }
    }
}