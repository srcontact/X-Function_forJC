using static I2.Loc.ScriptLocalization;
using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using System;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class AssessVariableFuncPar : BranchFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.assessVariable;

        public VariableDataNumericGet parameter1 = new() { useVariable = true };

        public VariableDataNumericGet parameter2V = new();

        public ComparatorType comparatorType;


        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (ComparatorType* ct = &comparatorType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_assessVariableFuncPar.parameter1, pgNodeParDescription_assessVariableFuncPar.parameter1);
                pgbepManager.SetPgbepVariable(parameter1, false);

                pgbepManager.SetHeaderText(pgNodeParameter_assessVariableFuncPar.parameter2, pgNodeParDescription_assessVariableFuncPar.parameter2);
                parameter2V.IndicateSwitchableFloat(pgbepManager);

                pgbepManager.SetHeaderText(pgNodeParameter_assessVariableFuncPar.comparator, pgNodeParDescription_assessVariableFuncPar.comparator);
                pgbepManager.SetPgbepEnum(typeof(ComparatorType), (int*)ct);
            }
        }

        public override bool BranchExecute(MachineLD ld)
        {
            var p1 = parameter1.GetUseValueInt(ld);
            var p2 = parameter2V.GetUseValueFloat(ld);

            switch (comparatorType)
            {
                case ComparatorType.EqualTo:
                    return Math.Abs(p1 - p2) < float.Epsilon;
                case ComparatorType.Over:
                    return p1 >= p2;
                case ComparatorType.GreaterThan:
                    return p1 > p2;
                case ComparatorType.Under:
                    return p1 <= p2;
                case ComparatorType.LessThan:
                    return p1 < p2;
                default:
                    return false;
            }
        }

        public override string[] GetNodeFaceText()
        {
            return new[] { $"[{parameter1.name}]\n{GetComparatorStr(comparatorType)}\n{parameter2V.GetIndicateStr()}" };
        }
    }
}