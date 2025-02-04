using System;
using clrev01.Menu;
using clrev01.PGE.NodeFace;
using clrev01.Programs;
using clrev01.Programs.FuncPar.Comment;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using System.Collections.Generic;
using clrev01.Bases;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static clrev01.Programs.UtlOfProgram;
using static I2.Loc.ScriptLocalization;

namespace clrev01.PGE.PGB
{
    public partial class PGBlock2 : MenuFunction
    {
        public int index { get; set; }
        public PGData EditPg { get; set; }
        public PGBData pgbd => EditPg?.pgList[index];
        public IPGBFuncUnion funcPar => pgbd?.funcPar;
        public PGBEditorPar editorPar => pgbd?.editorPar;

        public RectTransform rectTransform;
        public NodeFaceBasic nodeFace { get; set; }
        public Transform nodeFaceArea;
        public bool connected => nextIndex >= 0 && nextIndex != index && falseNextIndex >= 0 && falseNextIndex != index;

        private Camera _camera;
        private Image _image;
        private bool _currentVisibleState;

        protected override void Awake()
        {
            base.Awake();
            _camera = Camera.main;
            _image = GetComponent<Image>();
        }

        private void Update()
        {
            if (transform.hasChanged)
            {
                var visibleState = IsVisibleInViewport(rectTransform, _camera);
                if (visibleState != _currentVisibleState)
                {
                    _currentVisibleState = visibleState;
                    _image.enabled = visibleState;
                    nodeFaceArea.gameObject.SetActive(visibleState);
                }
                if (!visibleState) return;
                ConnectLineUpdate();
            }
            else transform.hasChanged = false;
        }

        public void DataSetting()
        {
            lpos = editorPar.EditorPos;
            scl = Vector3.one;
            nodeFace.baseOfCl.lpos = Vector3.zero;
            nodeFace.baseOfCl.scl = Vector3.one;
            nodeFace.SetPgbFunc(funcPar);
            var cbaSet = PCD.GetColorBlockAsset(funcPar);
            tgtButton.colorSetting = cbaSet.cba;
            tgtButton.toggledColorSetting = cbaSet.cbat;
            trueLine.gameObject.SetActive(funcPar is not CommentFuncPar);
            commentLine.gameObject.SetActive(funcPar is CommentFuncPar);
            falseLine.gameObject.SetActive(funcPar is BranchFuncPar);
            ConnectLineUpdate();
        }

        public override void ExeOnClick()
        {
            base.ExeOnClick();
            if (nowDrag) return;
            if (PGEM2.nowConnectionChangingPGB != null)
            {
                ConnectionChanging(PGEM2.nowConnectionChangingPGB, PGEM2.connectNum);
                PGEM2.nowConnectionChangingPGB = null;
            }
            else SelectGo();
        }
        public override void ExeOnRightClick()
        {
            base.ExeOnRightClick();
            if (PGEM2.nowConnectionChangingPGB != null) return;
            SelectThisOtherActions();
            PGEM2.quickMenuDialog.OpenQuickMenu(
                new List<string>()
                {
                    pgEditor.copy,
                    pgEditor.cut,
                    pgEditor.selectAllDownstream,
                    pgEditor.delete
                },
                QuickMenuAction);
        }
        void QuickMenuAction(int num)
        {
            switch (num)
            {
                case 0:
                    PGEM2.clipBoard.CopySelected();
                    PGEM2.multiSelect = false;
                    PGEM2.ResetSelectPgbs();
                    break;
                case 1:
                    PGEM2.clipBoard.CutSelected();
                    PGEM2.multiSelect = false;
                    PGEM2.ResetSelectPgbs();
                    break;
                case 2:
                    PGEM2.SelectAllDownstream();
                    break;
                case 3:
                    PGEM2.DeleteExe(PGEM2.selectPgbs);
                    PGEM2.multiSelect = false;
                    break;
            }
        }
        public override void ExeOnDrag(PointerEventData eventData)
        {
            base.ExeOnDrag(eventData);
            Debug.Log("OnDrag");
            DragMoveExe();
        }
        public override void ExeOnBeginDrag(PointerEventData eventData)
        {
            base.ExeOnBeginDrag(eventData);
            Debug.Log("OnBeginDrag");
            DragMoveStart();
            SelectThisOtherActions();
        }
        public override void ExeOnEndDrag(PointerEventData eventData)
        {
            base.ExeOnEndDrag(eventData);
            Debug.Log("OnEndDrag_DropTo=>" + eventData.pointerDrag);
            DragMoveEnd();
        }
        public override void ExeOnDrop(PointerEventData eventData)
        {
            base.ExeOnDrop(eventData);
            if (PGEM2.nowConnectionChangingPGB != null)
            {
                ConnectionChanging(PGEM2.nowConnectionChangingPGB, PGEM2.connectNum);
                PGEM2.nowConnectionChangingPGB = null;
            }
        }
    }
}