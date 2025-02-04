using clrev01.Menu;
using clrev01.Programs;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE
{
    public class MultiSelectButtonOnPGE : MenuFunction
    {
        [SerializeField]
        ColorBlockAsset selectedColorInfo;
        private bool currentMultiSelect;

        private void Update()
        {
            if (currentMultiSelect != PGEM2.multiSelect)
            {
                currentMultiSelect = PGEM2.multiSelect;
                ColorSetting();
            }
        }
        public override void ExeOnClick()
        {
            base.ExeOnClick();
            PGEM2.multiSelect = !PGEM2.multiSelect;
            PGEM2.ResetSelectPgbs();
            ColorSetting();
        }

        public void ColorSetting()
        {
            if (PGEM2.multiSelect) tgtButton.colorSetting = selectedColorInfo;
            else tgtButton.colorSetting = null;
        }
    }
}