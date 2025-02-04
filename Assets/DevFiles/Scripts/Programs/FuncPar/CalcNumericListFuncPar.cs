using static I2.Loc.ScriptLocalization;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.PGE.VariableEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using System;
using System.Collections.Generic;
using static clrev01.Save.UtlOfVariable;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable]
    public partial class CalcNumericListFuncPar : FunctionFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.calcNumericList;
        public VariableDataNumericList parameter1V = new();
        public VariableDataNumericGet parameter2V = new();
        public VariableDataNumericList targetVariable = new() { useVariable = true };
        public CalcNumericOperatorType operatorType;
        public ListCalcType listCalcType;

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            //todo:PGBEの表示を、計算の形が直感的にわかるように専用のパーツを使って表示するようにしたい
            fixed (CalcNumericOperatorType* opt = &operatorType)
            fixed (ListCalcType* lct = &listCalcType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_calcNumericListFuncPar.operatorType, pgNodeParDescription_calcNumericListFuncPar.operatorType);
                pgbepManager.SetPgbepEnum(typeof(CalcNumericOperatorType), (int*)opt);

                GetUseVariableFlags(out var usePar2);

                pgbepManager.SetHeaderText(pgNodeParameter_calcNumericListFuncPar.parameter1, pgNodeParDescription_calcNumericListFuncPar.parameter1);
                pgbepManager.SetPgbepVariable(parameter1V, false);
                if (usePar2)
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_calcNumericListFuncPar.parameter2, pgNodeParDescription_calcNumericListFuncPar.parameter2);
                    pgbepManager.SetPgbepEnum(typeof(ListCalcType), (int*)lct);
                    switch (listCalcType)
                    {
                        case ListCalcType.Element:
                            parameter2V.IndicateSwitchableFloat(pgbepManager, null, new List<VariableType> { VariableType.Numeric, VariableType.NumericList });
                            break;
                        case ListCalcType.List:
                            if (parameter2V.variableType is not VariableType.NumericList)
                            {
                                parameter2V = new VariableDataNumericGet() { useVariable = true, variableType = VariableType.NumericList };
                            }
                            pgbepManager.SetPgbepVariable(parameter2V, false, new List<VariableType> { VariableType.NumericList });
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                pgbepManager.SetHeaderText(pgNodeParameter_calcNumericListFuncPar.targetVariable, pgNodeParDescription_calcNumericListFuncPar.targetVariable);
                pgbepManager.SetPgbepVariable(targetVariable, false);
            }
        }

        private List<float> _resList = new();
        public void ExeCalcNumericList(MachineLD ld)
        {
            var p1L = parameter1V.GetUseValue(ld);
            _resList.Clear();
            GetUseVariableFlags(out var usePar2);
            switch (usePar2, listCalcType)
            {
                case (false, _):
                case (true, ListCalcType.Element):
                    var p2 = parameter2V.GetUseValueFloat(ld, null, null);
                    foreach (var p1 in p1L)
                    {
                        var res = CalcNumeric(ld, operatorType, p1, p2);
                        if (res != null) _resList.Add(res.Value);
                    }
                    break;
                case (true, ListCalcType.List):
                    var p2L = parameter2V.GetUseList(ld);
                    for (var i = 0; i < p1L.Count || i < p2L.Count; i++)
                    {
                        var res = CalcNumeric(ld, operatorType, i >= 0 && i < p1L.Count ? p1L[i] : 0, i >= 0 && i < p2L.Count ? p2L[i] : 0);
                        if (res != null) _resList.Add(res.Value);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            targetVariable.SetValue(ld, _resList);
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
                case CalcNumericOperatorType.Max:
                case CalcNumericOperatorType.Min:
                    usePar2 = false;
                    break;
                case CalcNumericOperatorType.Addition:
                case CalcNumericOperatorType.Subtraction:
                case CalcNumericOperatorType.Multiplication:
                case CalcNumericOperatorType.Division:
                case CalcNumericOperatorType.Modulo:
                case CalcNumericOperatorType.And:
                case CalcNumericOperatorType.Or:
                case CalcNumericOperatorType.Xor:
                case CalcNumericOperatorType.Random:
                    usePar2 = true;
                    break;
            }
        }

        public override string[] GetNodeFaceText()
        {
            var operatorStr = (CalcNumericOperatorType)operatorType switch
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
            var targetVariableName = targetVariable.GetIndicateStr();
            if (useParameter2)
            {
                var p2Str = parameter2V.GetIndicateStr(null, 1, listCalcType);
                return new[] { $"{parameter1V.GetIndicateStr()}\n{operatorStr}\n{p2Str}\n->\n{targetVariableName}" };
            }
            else return new[] { $"{parameter1V.GetIndicateStr()}\n{operatorStr}\n{targetVariableName}" };
        }
    }
}