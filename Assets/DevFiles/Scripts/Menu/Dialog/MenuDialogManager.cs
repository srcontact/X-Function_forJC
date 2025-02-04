using clrev01.Bases;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace clrev01.Menu.Dialog
{
    public class MenuDialogManager : BaseOfCL
    {
        [SerializeField, ReadOnly]
        Stack<MenuDialog> openDialogs = new Stack<MenuDialog>();
        [SerializeField]
        BackPanel backPanel;
        [SerializeField]
        List<MenuDialog> dialogs = new List<MenuDialog>();
        public SimpleDialog simpleDialog;
        public QuickMenuDialog quickMenuDialog;

        //ダイアログのヒエラルキー上の順序を開くたびに調整しなければならない。（新しいものが下にならないように。
        //バックは常に下から2番目にして、一番下（新しい）ダイアログ以外のメニューを隠すようにする。
        private void Awake()
        {
            CloseAllDialogs();
            backPanel.onClick.AddListener(CloseCurrentDialogOnClickBackPanel);
        }

        private void CloseCurrentDialogOnClickBackPanel()
        {
            var menuDialog = openDialogs.Peek();
            if (menuDialog.closeOnTouchBackFlag)
            {
                menuDialog.gameObject.SetActive(false);
            }
        }

        public void OnOpenDialog(MenuDialog dialog)
        {
            SetDialogHierarchy(dialog);
            openDialogs.Push(dialog);
            backPanel.gameObject.SetActive(true);
        }
        public void OnCloseDialog()
        {
            if (openDialogs.Count != 0) openDialogs.Pop();
            if (openDialogs.Count <= 0)
            {
                backPanel.gameObject.SetActive(false);
            }
        }
        void SetDialogHierarchy(MenuDialog dialog)
        {
            backPanel.transform.SetAsLastSibling();
            dialog.transform.SetAsLastSibling();
        }

        private void CloseAllDialogs()
        {
            for (int i = 0; i < dialogs.Count; i++)
            {
                dialogs[i].gameObject.SetActive(false);
            }
            backPanel.gameObject.SetActive(false);
        }

        private void OnValidate()
        {
            dialogs.Clear();
            dialogs.AddRange(GetComponentsInChildren<MenuDialog>(true));
        }

#if UNITY_EDITOR
        public void OpenSingle(MenuDialog dialog)
        {
            CloseAllDialogs();
            dialog.gameObject.SetActive(true);
        }
        [ContextMenu("DebugOpen")]
        public void DebugOpen()
        {
            gameObject.SetActive(true);
        }
#endif
    }
}