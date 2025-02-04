using clrev01.Bases;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace clrev01.ClAction.UI
{
    public class StatusBar : BaseOfCL
    {
        [SerializeField]
        TextMeshProUGUI text;
        [SerializeField]
        Image image;
        [SerializeField]
        private bool useMinValue;
        [SerializeField, ShowIf("useMinValue")]
        private int minValue = 0;
        [SerializeField]
        private bool limitMax;

        //public void SetIndicate(int max, int value)
        //{
        //    text.text = value.ToString();
        //    image.fillAmount = value / max;
        //}
        public void SetIndicate(float max, float value)
        {
            if (useMinValue) value = Mathf.Max(minValue, value);
            if (limitMax) value = Mathf.Min(max, value);
            text.text = value.ToString("0");
            image.fillAmount = value / max;
        }
    }
}