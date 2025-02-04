using clrev01.Extensions;
using clrev01.Menu;
using clrev01.Programs;
using clrev01.Save;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE
{
    public class ClipBoard : MenuFunction
    {
        [SerializeField]
        GameObject newPGBIcon;
        public List<PGBData> clipedPGDs = new List<PGBData>();
        List<int> clipNums = new List<int>();

        private void OnEnable()
        {
            tgtButtonInteractive = clipedPGDs.Count != 0;
        }
        public void CopySelected()
        {
            clipedPGDs.Clear();
            clipNums.Clear();
            Vector2 centerPos = PGEM2.currentClickedPGB.transform.localPosition;
            for (int i = 0; i < PGEM2.selectPgbs.Count; i++)
            {
                PGBData nd = PGEM2.selectPgbs[i].pgbd.CloneDeep();
                nd.editorPar.EditorPos -= centerPos;
                clipNums.Add(nd.editorPar.myIndex);
                clipedPGDs.Add(nd);
            }
            tgtButtonInteractive = clipedPGDs.Count != 0;
        }
        public void CutSelected()
        {
            CopySelected();
            PGEM2.DeleteExe(PGEM2.selectPgbs);
            PGEM2.multiSelect = false;
        }

        private int GetNextNum(List<int> newNums, int n, int mn)
        {
            if (clipNums.Contains(n))
            {
                for (int j = 0; j < clipNums.Count; j++)
                {
                    if (n == clipNums[j])
                    {
                        return newNums[j];
                    }
                }
            }
            return mn;
        }

        public override void ExeOnClick()
        {
            base.ExeOnClick();
            PGEM2.PastePGDs(PGEM2.pointerCursor.lpos, clipedPGDs);
        }
        public override void ExeOnBeginDrag(PointerEventData eventData)
        {
            base.ExeOnBeginDrag(eventData);
            newPGBIcon.SetActive(true);
            PGEM2.MoveTrackPointer(newPGBIcon.transform);
        }
        public override void ExeOnDrag(PointerEventData eventData)
        {
            base.ExeOnDrag(eventData);
            PGEM2.MoveTrackPointer(newPGBIcon.transform);
        }
        public override void ExeOnEndDrag(PointerEventData eventData)
        {
            base.ExeOnEndDrag(eventData);
            newPGBIcon.SetActive(false);
            PGEM2.PastePGDs(newPGBIcon.transform.localPosition, clipedPGDs);
        }
    }
}