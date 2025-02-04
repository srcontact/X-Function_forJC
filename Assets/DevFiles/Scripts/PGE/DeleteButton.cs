using clrev01.Menu;
using clrev01.PGE.PGB;
using clrev01.Programs;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE
{
    public class DeleteButton : MenuFunction
    {
        public override void ExeOnClick()
        {
            base.ExeOnClick();
            DeletePGBExe();
        }

        public override void ExeOnDrop(PointerEventData eventData)
        {
            base.ExeOnDrop(eventData);
            DeletePGBExe();
        }

        private static void DeletePGBExe()
        {
            if (PGEM2.currentClickedPGB != null)
            {
                if (PGEM2.multiSelect)
                {
                    PGEM2.DeleteExe(PGEM2.selectPgbs);
                    PGEM2.multiSelect = false;
                }
                else PGEM2.DeleteExe(new List<PGBlock2>() { PGEM2.currentClickedPGB });
                PGEM2.ResetSelectPgbs();
            }
        }

        //public void OnDrop(PointerEventData eventData)
        //{
        //    Debug.Log("DeleteButton_Droped++" + eventData.pointerDrag);
        //    PGEM2.DeleteExe(PGEM2.DragPGBD);
        //}
    }
}