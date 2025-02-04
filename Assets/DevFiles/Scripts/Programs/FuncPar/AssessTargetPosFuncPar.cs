using static I2.Loc.ScriptLocalization;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FieldPar;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save.VariableData;
using MemoryPack;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class AssessTargetPosFuncPar : BranchFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.assessTargetPos;
        public VariableDataLockOnGet targetList = new();
        public ISearchFieldUnion searchFieldPar = new BoxSearchFieldParVariable();

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            pgbepManager.SetHeaderText(pgNodeParameter_assessTargetPosFuncPar.target, pgNodeParDescription_assessTargetPosFuncPar.target);
            targetList.IndicateWithIndex(pgbepManager);
            pgbepManager.SetHeaderText(pgNodeParameter_assessTargetPosFuncPar.searchField, pgNodeParDescription_assessTargetPosFuncPar.searchField);
            pgbepManager.SetPgbepField(searchFieldPar, (res) => { searchFieldPar = (ISearchFieldUnion)res; });
        }

        public override bool BranchExecute(MachineLD ld)
        {
            var tgt = targetList.GetUseValue(ld);
            return tgt != null && searchFieldPar.AssessTgtPos(ld, ld.hd.transform, tgt.pos);
        }
        public override string[] GetNodeFaceText()
        {
            return new[] { $"{targetList.GetIndicateStr()} {searchFieldPar.GetFieldShortText()}" };
        }
        public override IFieldEditObject GetNodeFaceIFieldEditObject()
        {
            return searchFieldPar;
        }
    }
}