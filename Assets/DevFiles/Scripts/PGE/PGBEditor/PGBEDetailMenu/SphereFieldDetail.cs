using clrev01.Programs;
using clrev01.Programs.FieldPar;

namespace clrev01.PGE.PGBEditor.PGBEDetailMenu
{
    public class SphereFieldDetail : FieldDetailBase<ISphereFieldEditObject, SphereField3dIndicater>
    {
        protected override bool is2D => false;
        protected override UtlOfProgram.SearchFieldType FieldType => UtlOfProgram.SearchFieldType.Sphere;
    }
}