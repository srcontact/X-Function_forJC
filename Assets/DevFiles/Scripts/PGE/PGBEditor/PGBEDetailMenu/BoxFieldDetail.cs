using clrev01.Programs;
using clrev01.Programs.FieldPar;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE.PGBEditor.PGBEDetailMenu
{
    public class BoxFieldDetail : FieldDetailBase<IBoxFieldEditObject, BoxField3dIndicater>
    {
        protected override bool is2D => fieldPar.Is2D;
        protected override SearchFieldType FieldType => SearchFieldType.Box;
    }
}