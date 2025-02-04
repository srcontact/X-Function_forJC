using clrev01.Bases;
using TMPro;
using UnityEngine;

namespace clrev01.Menu
{
    public abstract class DataInformation<D> : BaseOfCL
        where D : new()
    {
        #region indicateData

        [SerializeField]
        private D _indicateData;
        public D indicateData
        {
            get { return _indicateData; }
            set
            {
                _indicateData = value;
                SettingIndicate();
            }
        }

        #endregion

        [SerializeField]
        TextMeshProUGUI text;
        public MenuButton button, deleteButton;
        public abstract string emptyText { get; }
        public abstract string infoText { get; }

        protected virtual void SettingIndicate()
        {
            string s;
            if (indicateData == null)
            {
                s = emptyText;
            }
            else
            {
                s = infoText;
            }
            text.text = s;
        }
    }
}