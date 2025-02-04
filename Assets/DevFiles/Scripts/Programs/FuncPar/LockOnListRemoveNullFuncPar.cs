using static I2.Loc.ScriptLocalization;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save.VariableData;
using MemoryPack;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable]
    public partial class LockOnListRemoveNullFuncPar : FunctionFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.lockonListRemoveNull;

        public VariableDataLockOnList targetList = new();

        public override void SetPointers(PgbepManager pgbepManager)
        {
            pgbepManager.SetHeaderText(pgNodeParameter_lockOnListRemoveNullFuncPar.targetList, pgNodeParDescription_lockOnListRemoveNullFuncPar.targetList);
            pgbepManager.SetPgbepVariable(targetList, false);
        }

        public void ExeLockOnListRemoveNull(MachineLD ld)
        {
            targetList.RemoveNull(ld);
        }
    }
}