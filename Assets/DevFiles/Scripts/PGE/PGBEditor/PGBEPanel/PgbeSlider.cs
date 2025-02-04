using UnityEngine;
using UnityEngine.UI;

namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public class PgbeSlider : PgbePanel
    {
        [SerializeField]
        Slider slider;
        public Slider Slider => slider;

        protected override void ResetTgtData()
        {
            slider.onValueChanged.RemoveAllListeners();
        }
    }
}