using clrev01.Bases;
using clrev01.Extensions;
using clrev01.Programs;
using System.Collections.Generic;
using UnityEngine;
using static clrev01.Extensions.ExUI;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE.PGB
{
    public partial class PGBlock2
    {
        public bool nowDrag;
        public Vector3 currentMouse, startTgt;

        void DragMoveStart()
        {
            nowDrag = true;
            SetDragStartPar();
            if (!PGEM2.multiSelect) return;
            foreach (var selectPgb in PGEM2.selectPgbs)
            {
                selectPgb.SetDragStartPar();
            }
        }

        private void SetDragStartPar()
        {
            currentMouse = GetPointerPos();
            startTgt = transform.position;
        }

        void DragMoveExe()
        {
            Vector3 v = GetPointerPos() - currentMouse;
            UpdateDragMovePos(v);
            if (!PGEM2.multiSelect) return;
            foreach (var selectPgb in PGEM2.selectPgbs)
            {
                selectPgb.UpdateDragMovePos(v);
            }
        }

        private void UpdateDragMovePos(Vector3 v)
        {
            transform.position = startTgt + v;
        }

        private void DragMoveEnd()
        {
            nowDrag = false;
            UpdateEditorPos();
            var updateIndexes = new List<int> { editorPar.myIndex };
            if (PGEM2.multiSelect)
            {
                foreach (var selected in PGEM2.selectPgbs)
                {
                    selected.UpdateEditorPos();
                }
                updateIndexes.AddRange(PGEM2.selectPgbs.ConvertAll(x => x.editorPar.myIndex));
            }
            var isValid = StaticInfo.Inst.UndoManager.UpdatePgbdStart();
            StaticInfo.Inst.UndoManager.UpdatePgbdLog(isValid, updateIndexes);
            StaticInfo.Inst.UndoManager.UpdatePgbdEnd(isValid);
        }

        public void UpdateEditorPos()
        {
            editorPar.EditorPos = lpos;
            lpos = editorPar.EditorPos;
        }
    }
}