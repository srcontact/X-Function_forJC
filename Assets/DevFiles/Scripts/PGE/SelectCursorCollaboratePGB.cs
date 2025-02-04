using clrev01.Menu;
using clrev01.Programs;
using UnityEngine.EventSystems;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE
{
    public class SelectCursorCollaboratePGB : MenuFunction
    {
        public override void ExeOnClick()
        {
            base.ExeOnClick();
            PGEM2.currentClickedPGB.ExeOnClick();
        }
        public override void ExeOnDoubleClick()
        {
            base.ExeOnDoubleClick();
            PGEM2.currentClickedPGB.ExeOnDoubleClick();
        }
        public override void ExeOnRightClick()
        {
            base.ExeOnRightClick();
            PGEM2.currentClickedPGB.ExeOnRightClick();
        }
        public override void ExeOnDrop(PointerEventData eventData)
        {
            base.ExeOnDrop(eventData);
            PGEM2.currentClickedPGB.ExeOnDrop(eventData);
        }
        public override void ExeOnBeginDrag(PointerEventData eventData)
        {
            base.ExeOnBeginDrag(eventData);
            PGEM2.currentClickedPGB.ExeOnBeginDrag(eventData);
        }
        public override void ExeOnDrag(PointerEventData eventData)
        {
            base.ExeOnDrag(eventData);
            PGEM2.currentClickedPGB.ExeOnDrag(eventData);
        }
        public override void ExeOnEndDrag(PointerEventData eventData)
        {
            base.ExeOnEndDrag(eventData);
            PGEM2.currentClickedPGB.ExeOnEndDrag(eventData);
        }
    }
}