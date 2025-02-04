using static I2.Loc.ScriptLocalization;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using static clrev01.Save.UtlOfVariable;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable]
    public partial class ManageLockOnFuncPar : FunctionFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.manageLockon;
        public VariableDataLockOnGet parameter1 = new();
        public VariableDataLockOnSet targetVariable = new();
        public ManageLockOnOperatorType operatorType;

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (ManageLockOnOperatorType* opt = &operatorType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_manageLockOnFuncPar.operatorType, pgNodeParDescription_manageLockOnFuncPar.operatorType);
                pgbepManager.SetPgbepEnum(typeof(ManageLockOnOperatorType), (int*)opt);

                pgbepManager.SetHeaderText(pgNodeParameter_manageLockOnFuncPar.parameter1, pgNodeParDescription_manageLockOnFuncPar.parameter1);
                parameter1.IndicateWithIndex(pgbepManager);

                pgbepManager.SetHeaderText(pgNodeParameter_manageLockOnFuncPar.targetVariable, pgNodeParDescription_manageLockOnFuncPar.targetVariable);
                targetVariable.IndicateWithIndex(pgbepManager);
            }
        }

        public void ExeManageLockOn(MachineLD ld)
        {
            var res = ManageLockOn(ld, operatorType, parameter1.GetUseValue(ld));
            targetVariable.SetLockOn(ld, res);
        }
    }
}