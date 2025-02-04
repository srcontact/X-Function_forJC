using clrev01.Bases;
using clrev01.Menu.Dialog;
using clrev01.Menu.EditMenu;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace clrev01.Menu
{
    public class MenuPagePanelManager : SingletonOfCL<MenuPagePanelManager>
    {
        [SerializeField]
        TextMeshProUGUI titleText, captionText;
        [SerializeField]
        Button returnButton;
        [SerializeField]
        MenuPage startPage;
        public List<MenuPage> PageList = new List<MenuPage>();
        [ReadOnly]
        public int nowPage
        {
            get { return StaticInfo.Inst.nowPage; }
            set { StaticInfo.Inst.nowPage = value; }
        }
        [ReadOnly]
        public List<int> pageLog
        {
            get { return StaticInfo.Inst.pageLog; }
            set { StaticInfo.Inst.pageLog = value; }
        }
        public MenuDialogManager dialogManager;
        public EditMenuActiveCont editMenuActiveCont;
        public MouseOverTips mouseOverTips;

        public override void Awake()
        {
            base.Awake();
            returnButton.onClick.AddListener(ReturnPage);
            if (nowPage > -1) SetPageActive(PageList[nowPage]);
            else SetPageActive(startPage);
            returnButton.colors = StaticInfo.Inst.normalColorInfo.colorBlock;
            editMenuActiveCont.ActivateExe();
        }

        public void OpenPage(MenuPage menuPage)
        {
            LoggingPage(menuPage);
            SetPageActive(menuPage);
        }
        public void ReturnPage()
        {
            MenuPage tgt = PageList[pageLog[pageLog.Count - 1]];
            pageLog.RemoveAt(pageLog.Count - 1);
            SetPageActive(tgt);
        }
        public void SetPageTitle(string title)
        {
            if (title == string.Empty) return;
            titleText.text = title;
        }
        public void SetCaptionText(string caption)
        {
            captionText.text = caption;
        }

        public void SetPageActive(MenuPage menuPage)
        {
            foreach (var page in PageList)
            {
                page.PageDisable();
            }
            menuPage.PageEnable();
            nowPage = PageList.IndexOf(menuPage);
            returnButton.gameObject.SetActive(pageLog.Count > 0);
        }
        void LoggingPage(MenuPage menuPage)
        {
            pageLog.Add(nowPage);
        }

        private void OnValidate()
        {
            ResetPageList();
        }
        public void ResetPageList()
        {
            PageList.Clear();
            PageList.AddRange(GetComponentsInChildren<MenuPage>(true));
        }
    }
}