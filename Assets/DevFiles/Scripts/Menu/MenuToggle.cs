using clrev01.Bases;
using System;
using TMPro;
using UnityEngine;

namespace clrev01.Menu
{
    public class MenuToggle : BaseOfCL
    {
        #region togglePar
        [SerializeField]
        private bool _togglePar;
        public bool togglePar
        {
            get { return _togglePar; }
            set
            {
                _togglePar = value;
                if (toggleEvent != null) toggleEvent.Invoke(togglePar);
                trueObj.SetActive(togglePar);
                falseObj.SetActive(!togglePar);
                if (txt != null) txt.text = togglePar ? trueStr : falseStr;
            }
        }
        public bool setToggleNoEvent
        {
            get => _togglePar;
            set
            {
                _togglePar = value;
                trueObj.SetActive(togglePar);
                falseObj.SetActive(!togglePar);
                if (txt != null) txt.text = togglePar ? trueStr : falseStr;
            }
        }
        #endregion
        [SerializeField]
        MenuButton menuButton;
        [SerializeField]
        TextMeshProUGUI txt;
        [SerializeField]
        GameObject trueObj, falseObj;
        [SerializeField]
        string trueStr = "True", falseStr = "False";
        public Action<bool> toggleEvent;

        private void Awake()
        {
            menuButton.OnClick.AddListener(() => OnClick());
        }
        private void OnClick()
        {
            togglePar = !togglePar;
        }
        private void OnValidate()
        {
            togglePar = togglePar;
        }
    }
}