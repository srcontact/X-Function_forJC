using System;
using UnityEngine;
using UnityEngine.UI;

namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public class PgbeFloatPanel : PgbeUnmanagedInputPanel<float>
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
        public unsafe void SetSlider(Slider slider, float min, float max, int unit)
        {
            this.slider = slider;
            this.slider.minValue = min;
            this.slider.maxValue = max;
            this.slider.wholeNumbers = unit == 0 ? true : false;
            this.slider.SetValueWithoutNotify(*tgtPointer);
            this.slider.onValueChanged.AddListener((float f) =>
            {
                float ff = (float)Math.Round(f, unit);
                SetIndicate(ff);
                *tgtPointer = ff;
            });
        }
    }
}