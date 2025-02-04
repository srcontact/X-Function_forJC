using static I2.Loc.ScriptLocalization;
using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using MemoryPack;
using static clrev01.Bases.UtlOfCL;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class AssessSelfActionFuncPar : BranchFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.assessSelfAction;
        public AssessActionType actionState;
        public long number = long.MaxValue;

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (AssessActionType* acs = &actionState)
            fixed (long* nu = &number)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_assessSelfActionFuncPar.actionState, pgNodeParDescription_assessSelfActionFuncPar.actionState);
                pgbepManager.SetPgbepEnum(typeof(AssessActionType), (int*)acs);
                switch (actionState)
                {
                    case AssessActionType.Fire:
                        pgbepManager.SetHeaderText(pgNodeParameter_assessSelfActionFuncPar.weaponNumber, pgNodeParDescription_assessSelfActionFuncPar.weaponNumber);
                        pgbepManager.SetPgbepSelectFlagsOption(nu, pgbepManager.GetEquipmentList());
                        break;
                    case AssessActionType.UseOptionalParts:
                        pgbepManager.SetHeaderText(pgNodeParameter_assessSelfActionFuncPar.optionalParts, pgNodeParDescription_assessSelfActionFuncPar.optionalParts);
                        pgbepManager.SetPgbepSelectFlagsOption(nu, pgbepManager.GetOptionalList());
                        break;
                }
            }
        }

        public override bool BranchExecute(MachineLD ld)
        {
            return ld.AssessAction(actionState, number);
        }

        public override string[] GetNodeFaceText()
        {
            if (actionState is AssessActionType.Fire)
            {
                string fireNumberStr = "";
                bool setSeparator = false, allSelected = true;
                var weaponCount = MHUB.GetData(PGEM2.nowEditCD.mechCustom.machineCode).machineCD.usableWeapons.Count;
                for (int i = 0; i < weaponCount; i++)
                {
                    if (((1 << i) & number) == 0)
                    {
                        allSelected = false;
                    }
                    else
                    {
                        fireNumberStr += $"{(setSeparator ? "," : "")}{i + 1}";
                        setSeparator = true;
                    }
                }
                return new[] { $"{actionState}{(allSelected ? "" : $"[{fireNumberStr}]")}" };
            }
            else
            {
                return new[] { $"{actionState}" };
            }
        }
    }
}