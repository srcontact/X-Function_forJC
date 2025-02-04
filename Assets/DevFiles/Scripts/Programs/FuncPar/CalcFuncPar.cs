using static I2.Loc.ScriptLocalization;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using System;
using static clrev01.Save.UtlOfVariable;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class CalcFuncPar : FunctionFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.calcNumeric;
        public VariableDataNumericGet parameter1V = new();
        public VariableDataNumericGet parameter2V = new();
        public VariableDataNumericSet targetVariable = new() { useVariable = true };
        public CalcNumericOperatorType operatorType;

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            //todo:PGBEの表示を、計算の形が直感的にわかるように専用のパーツを使って表示するようにしたい
            fixed (CalcNumericOperatorType* opt = &operatorType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_calcFuncPar.operatorType, pgNodeParDescription_calcFuncPar.operatorType);
                pgbepManager.SetPgbepEnum(typeof(CalcNumericOperatorType), (int*)opt);

                GetUseVariableFlags(out var usePar2);

                pgbepManager.SetHeaderText(pgNodeParameter_calcFuncPar.parameter1, pgNodeParDescription_calcFuncPar.parameter1);
                parameter1V.IndicateSwitchableFloat(pgbepManager);
                if (usePar2)
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_calcFuncPar.parameter2, pgNodeParDescription_calcFuncPar.parameter2);
                    parameter2V.IndicateSwitchableFloat(pgbepManager);
                }
                pgbepManager.SetHeaderText(pgNodeParameter_calcFuncPar.targetVariable, pgNodeParDescription_calcFuncPar.targetVariable);
                targetVariable.IndicateSwitchable(pgbepManager);
            }
        }
        public void GetUseVariableFlags(out bool usePar2)
        {
            usePar2 = false;
            switch (operatorType)
            {
                case CalcNumericOperatorType.Assignment:
                case CalcNumericOperatorType.RoundedDown:
                case CalcNumericOperatorType.Absolute:
                case CalcNumericOperatorType.Square:
                case CalcNumericOperatorType.Sin:
                case CalcNumericOperatorType.Cos:
                case CalcNumericOperatorType.Tan:
                case CalcNumericOperatorType.Atan:
                case CalcNumericOperatorType.Not:
                    usePar2 = false;
                    break;
                case CalcNumericOperatorType.Addition:
                case CalcNumericOperatorType.Subtraction:
                case CalcNumericOperatorType.Multiplication:
                case CalcNumericOperatorType.Division:
                case CalcNumericOperatorType.Modulo:
                case CalcNumericOperatorType.Max:
                case CalcNumericOperatorType.Min:
                case CalcNumericOperatorType.And:
                case CalcNumericOperatorType.Or:
                case CalcNumericOperatorType.Xor:
                case CalcNumericOperatorType.Random:
                    usePar2 = true;
                    break;
            }
        }

        public void ExeCalcNumeric(MachineLD ld)
        {
            var p1 = parameter1V.GetUseValueFloat(ld);
            var p2 = parameter2V.GetUseValueFloat(ld);
            var res = CalcNumeric(ld, operatorType, p1, p2);
            if (res.HasValue) targetVariable.SetNumericValue(ld, res.Value);
        }

        public override string[] GetNodeFaceText()
        {
            var operatorStr = operatorType switch
            {
                CalcNumericOperatorType.Assignment => "->",
                CalcNumericOperatorType.Addition => "+",
                CalcNumericOperatorType.Subtraction => "-",
                CalcNumericOperatorType.Multiplication => "×",
                CalcNumericOperatorType.Division => "÷",
                CalcNumericOperatorType.Modulo => "%",
                CalcNumericOperatorType.RoundedDown => "RoundedDown",
                CalcNumericOperatorType.Absolute => "Abs",
                CalcNumericOperatorType.Max => "Max",
                CalcNumericOperatorType.Min => "Min",
                CalcNumericOperatorType.Square => "Sqr",
                CalcNumericOperatorType.Sin => "Sin",
                CalcNumericOperatorType.Cos => "Cos",
                CalcNumericOperatorType.Tan => "Tan",
                CalcNumericOperatorType.Atan => "Atan",
                CalcNumericOperatorType.Not => "Not",
                CalcNumericOperatorType.And => "And",
                CalcNumericOperatorType.Or => "Or",
                CalcNumericOperatorType.Xor => "Xor",
                CalcNumericOperatorType.Random => "Rnd",
                _ => throw new ArgumentOutOfRangeException()
            };
            GetUseVariableFlags(out var useParameter2);
            var targetVariableName = targetVariable.GetIndicateStr(null);
            if (useParameter2) return new[] { $"{parameter1V.GetIndicateStr(null)}\n{operatorStr}\n{parameter2V.GetIndicateStr(null)}\n->\n{targetVariableName}" };
            else return new[] { $"{parameter1V.GetIndicateStr()}\n{operatorStr}\n{targetVariableName}" };
        }
    }
}