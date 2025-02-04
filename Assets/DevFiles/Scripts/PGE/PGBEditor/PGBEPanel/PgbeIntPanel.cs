using UnityEngine;
using UnityEngine.UI;

namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public class PgbeIntPanel : PgbeUnmanagedInputPanel<float>
    {
        [SerializeField]
        Slider slider;

        protected override void SetIndicate(float data)
        {
            inputField.text = data.ToString();
        }
        protected override bool TryParseExe(string s, out float res)
        {
            return float.TryParse(s, out res);
        }
        public unsafe void SetSlider(Slider slider, int min, int max)
        {
            this.slider = slider;
            this.slider.minValue = min;
            this.slider.maxValue = max;
            this.slider.wholeNumbers = true;
            this.slider.SetValueWithoutNotify(*tgtPointer);
            this.slider.onValueChanged.AddListener((float f) =>
            {
                int i = Mathf.RoundToInt(f);
                SetIndicate(i);
                *tgtPointer = i;
            });
        }
    }
}