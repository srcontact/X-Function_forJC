using clrev01.Bases;
using clrev01.Extensions;
using clrev01.Menu;
using clrev01.Menu.Dialog;
using clrev01.Programs;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE.PGBEditor
{
    public class PGBEditor : BaseOfCL
    {
        int index { get; set; }
        PGData editPG { get; set; }
        public PGBData pGBDataOrig
        {
            get => editPG.pgList[index];
            set => editPG.pgList[index] = value;
        }
        [NonSerialized]
        public PGBData pgbDataCopy;
        [SerializeField]
        GameObject editorBaseObj;
        [SerializeField]
        BackPanel editorBackPanel;
        [SerializeField]
        MenuButton acceptButton;
        [SerializeField]
        MenuButton typeButton;
        [SerializeField]
        TextMeshProUGUI typeText;
        [SerializeField]
        PgbepManager pgbepManager;
        [SerializeField]
        public ScrollRect scrollRect;
        [SerializeField]
        private ContentSizeFitter contentSizeFitter;
        private Action _afterEdit;

        private void Awake()
        {
            pgbepManager.initOnEditAction = () => InitializeEditPanels(true);
            editorBackPanel.onClick.AddListener(() => CloseEditor(false));
            acceptButton.OnClick.AddListener(() => CloseEditor(true));
            typeButton.OnClick.AddListener(() => OpenSelector(typeButton));
        }
        public void OpenEditor(int index, PGData editPG, Action afterEditAction = null)
        {
            this.index = index;
            this.editPG = editPG;
            pgbDataCopy = pGBDataOrig.CloneDeep();
            gameObject.SetActive(true);
            InitializeEditPanels(false);
            if (afterEditAction != null) _afterEdit += afterEditAction;
        }

        public void CloseEditor(bool acceptFlag)
        {
            if (!gameObject.activeSelf) return;
            editorBaseObj.SetActive(false);
            if (acceptFlag)
            {
                pGBDataOrig = pgbDataCopy;
                var isValid = StaticInfo.Inst.UndoManager.UpdatePgbdStart();
                StaticInfo.Inst.UndoManager.UpdatePgbdLog(isValid, new List<int>() { pGBDataOrig.editorPar.myIndex });
                StaticInfo.Inst.UndoManager.UpdatePgbdEnd(isValid);
                pgbepManager.ResetPgbeps(false);
            }
            if (_afterEdit != null) _afterEdit.Invoke();
            _afterEdit = null;
            PGEM2.PGBSetting();
        }
        public unsafe void InitializeEditPanels(bool keepScroll)
        {
            pgbepManager.ResetPgbeps(keepScroll);

            pgbDataCopy.funcPar.SetPointers(pgbepManager);
            contentSizeFitter.SetLayoutVertical();
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 1;
            typeText.text = pgbDataCopy.funcPar.BlockTypeStr;
        }
        void OnFuncTypeValueChanged(int num)
        {
            int nn = pgbDataCopy.funcPar.GetFuncParNum;
            if (nn == num) return;
            pgbDataCopy.funcPar = PGBFuncPar.GetFuncParInstance(num);
            InitializeEditPanels(false);
        }
        void OpenSelector(MenuButton button)
        {
            Vector3 sp = Camera.main.WorldToScreenPoint(button.transform.position);
            PGEM2.OpenPgbTypeSelector(sp, (i) => OnFuncTypeValueChanged(i));
        }
    }
}