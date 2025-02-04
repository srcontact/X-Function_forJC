using clrev01.Bases;
using System;
using TMPro;

namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public abstract class PgbePanel : BaseOfCL
    {
        public TextMeshProUGUI titleLabel;
        [NonSerialized]
        public Action initPGBEPM;

        public void OnReset()
        {
            ResetTgtData();
            gameObject.SetActive(false);
        }
        protected abstract void ResetTgtData();
    }
}