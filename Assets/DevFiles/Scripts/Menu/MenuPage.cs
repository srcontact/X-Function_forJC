using clrev01.Bases;
using System.Collections.Generic;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Menu
{
    public class MenuPage : BaseOfCL
    {
#if UNITY_EDITOR
        [SerializeField]
        bool doOtherPageOff;
#endif
        public string pageTitle;
        public List<GameObject> setObjList = new();

        public void PageEnable()
        {
            foreach (var o in setObjList)
            {
                o.SetActive(true);
            }
            gameObject.SetActive(true);
            MPPM.SetPageTitle(pageTitle);
            MPPM.SetCaptionText(string.Empty);
        }

        public void PageDisable()
        {
            foreach (var o in setObjList)
            {
                o.SetActive(false);
            }
            gameObject.SetActive(false);
        }

        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            if (doOtherPageOff)
            {
                doOtherPageOff = false;
                MPPM.ResetPageList();
                MPPM.SetPageActive(this);
            }
#endif
        }
    }
}