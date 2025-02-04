using static I2.Loc.ScriptLocalization;
using clrev01.PGE.PGBEditor;
using clrev01.PGE.PGEM;
using clrev01.Programs.FuncPar.Comment;
using clrev01.Programs.FuncPar.FuncParType;
using MemoryPack;
using System.Linq;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class SubroutineRootFuncPar : SubroutineFuncPar
    {
        public override string BlockTypeStr => pgNodeName.subroutineRoot;

        public StringRefObj subroutineName = new("Subroutine_001");

        public override bool IsConnectable(IPGBFuncUnion connectionFrom)
        {
            return connectionFrom is CommentFuncPar;
        }

        public override void SetPointers(PgbepManager pgbepManager)
        {
            pgbepManager.SetHeaderText(pgNodeParameter_subroutineRootFuncPar.subroutineName, pgNodeParDescription_subroutineRootFuncPar.subroutineName);
            Format(pgbepManager);
            pgbepManager.SetPgbepString(subroutineName, () => Format(pgbepManager));
        }
        private void Format(PgbepManager pgbepManager)
        {
            //ここで名前被りを検証修正するが、ここで編集してるのはクローンのデータなので、元のPGList全てと検証するとオリジナルと被ってしまう。
            //そのため、ワザワザpgbepManager経由でオリジナルを持ってきてPGlistからそれを除いて検証している。
            PGEManager.Inst.nowEditPD.SubroutineNameRepetitionCorrection(
                PGEManager.Inst.nowEditPD.pgList
                    .Where(x => x is { funcPar: SubroutineRootFuncPar } && !ReferenceEquals(x, pgbepManager.editDataOrig))
                    .ToList()
                    .ConvertAll(x => (SubroutineRootFuncPar)x.funcPar),
                new[] { this }
            );
        }

        public override string[] GetNodeFaceText()
        {
            return new[] { subroutineName.obj };
        }
    }
}