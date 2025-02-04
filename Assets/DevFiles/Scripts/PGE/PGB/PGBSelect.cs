using clrev01.Programs;
using System.Linq;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE.PGB
{
    public partial class PGBlock2
    {
        public virtual void SelectGo()
        {
            if (!PGEM2.multiSelect && PGEM2.currentClickedPGB == this) EditGo();
            else SelectThisNormalClick();
        }
        public void SelectThisNormalClick()
        {
            if (PGEM2.multiSelect)
            {
                if (PGEM2.selectPgbs.Contains(this)) DeselectThis();
                else SelectThis();
            }
            else SelectThisSimple();
        }
        public void SelectThisSimple()
        {
            PGEM2.ResetSelectPgbs();
            PGEM2.multiSelect = false;
            SelectThis();
        }
        public void SelectThisOtherActions()
        {
            if (!PGEM2.multiSelect) PGEM2.ResetSelectPgbs();
            SelectThis();
        }
        public void SelectThis()
        {
            if (!PGEM2.selectPgbs.Contains(this)) PGEM2.selectPgbs.Add(this);
            if (PGEM2.selectPgbs.Last() == this && !tgtButton.isHighlight) tgtButton.isHighlight = true;
            PGEM2.ConnectButtonActiveUpdate();
        }
        public void DeselectThis()
        {
            if (PGEM2.selectPgbs.Contains(this)) PGEM2.selectPgbs.Remove(this);
            if (tgtButton.isHighlight) tgtButton.isHighlight = false;
            PGEM2.ConnectButtonActiveUpdate();
        }
        public void ReselectThis()
        {
            DeselectThis();
            SelectThis();
        }
    }
}