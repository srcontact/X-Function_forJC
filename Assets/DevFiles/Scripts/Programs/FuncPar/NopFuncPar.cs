using static I2.Loc.ScriptLocalization;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using MemoryPack;

namespace clrev01.Programs.FuncPar
{
    [MemoryPackable()]
    public partial class NopFuncPar : FunctionFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.nop;
        public override int calcCost => 0;
        public override bool OpenEditorOnCreateNode => false;
        public override void SetPointers(PgbepManager pgbepManager)
        { }
    }
}