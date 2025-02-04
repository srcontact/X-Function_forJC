using clrev01.Menu;
using clrev01.Programs;
using UnityEngine;
using UnityEngine.EventSystems;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE
{
    public class NewButton2 : MenuFunction
    {
        [SerializeField]
        GameObject newPGBIcon;

        public override void ExeOnClick()
        {
            base.ExeOnClick();
            PGEM2.CreateNewPGD(PGEM2.pointerCursor.lpos, Camera.main.WorldToScreenPoint(pos));
        }

        public override void ExeOnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("NewButton_BeginDrag");
            newPGBIcon.SetActive(true);
            PGEM2.MoveTrackPointer(newPGBIcon.transform);
        }

        public override void ExeOnDrag(PointerEventData eventData)
        {
            base.ExeOnDrag(eventData);
            Debug.Log("NewButton_Drag");
            PGEM2.MoveTrackPointer(newPGBIcon.transform);
        }

        public override void ExeOnEndDrag(PointerEventData eventData)
        {
            base.ExeOnEndDrag(eventData);
            Debug.Log("NewButton_EndDrag");
            newPGBIcon.SetActive(false);
            PGEM2.CreateNewPGD(newPGBIcon.transform.localPosition, Camera.main.WorldToScreenPoint(newPGBIcon.transform.position));
        }
    }
}