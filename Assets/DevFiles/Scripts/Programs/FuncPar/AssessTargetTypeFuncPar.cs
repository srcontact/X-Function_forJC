using static I2.Loc.ScriptLocalization;
using clrev01.ClAction.Machines;
using clrev01.ClAction.ObjectSearch;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save.VariableData;
using MemoryPack;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class AssessTargetTypeFuncPar : BranchFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.assessTargetType;
        public VariableDataLockOnGet targetList = new();
        public SearchTgtType targetType = SearchTgtType.Machine;

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (SearchTgtType* tt = &targetType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_assessTargetTypeFuncPar.target, pgNodeParDescription_assessTargetTypeFuncPar.target);
                targetList.IndicateWithIndex(pgbepManager);
                pgbepManager.SetHeaderText(pgNodeParameter_assessTargetTypeFuncPar.targetType, pgNodeParDescription_assessTargetTypeFuncPar.targetType);
                pgbepManager.SetPgbepFlagsEnum(typeof(SearchTgtType), (long*)tt);
            }
        }
        public override bool BranchExecute(MachineLD ld)
        {
            var tgt = targetList.GetUseValue(ld);
            if (tgt == null) return false;
            return (tgt.ObjectSearchType & targetType) > 0;
        }
        public override string[] GetNodeFaceText()
        {
            return new[] { $"{targetList.GetIndicateStr()}\nTgtT:{targetType}" };
        }
    }
}