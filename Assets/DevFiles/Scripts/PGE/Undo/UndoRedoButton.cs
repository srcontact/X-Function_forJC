using clrev01.Bases;
using clrev01.Menu;
using clrev01.Programs;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE.Undo
{
    public class UndoRedoButton : MenuFunction
    {
        [SerializeField]
        bool isRedo;
        public override void ExeOnClick()
        {
            base.ExeOnClick();
            if (!isRedo) StaticInfo.Inst.UndoManager.ExecuteUndo();
            else StaticInfo.Inst.UndoManager.ExecuteRedo();
            PGEM2.PGBSetting();
        }
    }
}