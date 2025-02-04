using static I2.Loc.ScriptLocalization;
using BurstLinq;
using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save.VariableData;
using MemoryPack;
using System;
using System.Linq;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Programs.FuncPar
{
    [Serializable]
    [MemoryPackable()]
    public partial class AssessTargetActionFuncPar : BranchFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.assessTargetAction;
        public VariableDataLockOnGet targetList = new();
        public AssessActionType actionState;
        public long number = long.MaxValue;

        private int weaponCount => MHUB.datas.Max(x => x.machineCD.usableWeapons.Count);

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (AssessActionType* acs = &actionState)
            fixed (long* nu = &number)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_assessTargetActionFuncPar.target, pgNodeParDescription_assessTargetActionFuncPar.target);
                targetList.IndicateWithIndex(pgbepManager);
                pgbepManager.SetHeaderText(pgNodeParameter_assessTargetActionFuncPar.actionState, pgNodeParDescription_assessTargetActionFuncPar.actionState);
                pgbepManager.SetPgbepEnum(typeof(AssessActionType), (int*)acs);
                if (actionState == AssessActionType.Fire)
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_assessTargetActionFuncPar.weaponNumber, pgNodeParDescription_assessTargetActionFuncPar.weaponNumber);
                    pgbepManager.SetPgbepSelectFlagsOption(nu, Enumerable.Range(1, weaponCount).ToList().ConvertAll(x => $"Weapon{x}"));
                }
            }
        }

        public override bool BranchExecute(MachineLD ld)
        {
            var tgt = targetList.GetUseValue(ld);
            if (tgt == null) return false;
            switch (tgt.hardBase)
            {
                case MachineHD mhd:
                    return mhd.ld.AssessAction(actionState, number);
                default:
                    return false;
            }
        }

        public override string[] GetNodeFaceText()
        {
            var targetStr = targetList.GetIndicateStr();
            if (actionState is AssessActionType.Fire)
            {
                string fireNumberStr = "";
                bool setSeparator = false, allSelected = true;
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
                return new[] { $"{targetStr} {actionState}{(allSelected ? "" : $"[{fireNumberStr}]")}" };
            }
            else
            {
                return new[] { $"{targetStr} {actionState}" };
            }
        }
    }
}