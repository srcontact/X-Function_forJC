using static I2.Loc.ScriptLocalization;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.Comment;
using clrev01.Programs.FuncPar.FuncParType;
using MemoryPack;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class StartFuncPar : PGBFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.start;

        public override bool IsConnectable(IPGBFuncUnion connectionFrom)
        {
            return connectionFrom is CommentFuncPar;
        }

        public override void SetPointers(PgbepManager pgbepManager)
        { }
    }
}