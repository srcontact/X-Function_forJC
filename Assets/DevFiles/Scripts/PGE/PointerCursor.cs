using clrev01.Bases;
using clrev01.Programs;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE
{
    public class PointerCursor : BaseOfCL
    {
        public void SetPosition()
        {
            PGEM2.MoveTrackPointer(transform);
        }
    }
}