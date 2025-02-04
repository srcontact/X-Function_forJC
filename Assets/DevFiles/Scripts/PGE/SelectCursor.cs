using clrev01.Bases;
using clrev01.PGE.PGB;
using clrev01.Programs;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE
{
    public class SelectCursor : BaseOfCL
    {
        [SerializeField]
        private RectTransform cursorRect;
        [SerializeField]
        private Vector3 posOffset = new(0, 0, -2);
        private PGBlock2 tgt => PGEM2 == null ? null : PGEM2.currentClickedPGB;

        private void Update()
        {
            if (tgt != null && tgt.gameObject.activeSelf)
            {
                if (!cursorRect.gameObject.activeSelf)
                {
                    cursorRect.gameObject.SetActive(true);
                }
                cursorRect.transform.position = tgt.transform.position;
                cursorRect.localPosition += posOffset;
            }
            else if (cursorRect.gameObject.activeSelf)
            {
                cursorRect.gameObject.SetActive(false);
            }
        }
    }
}