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
using UnityEngine;
using static clrev01.Save.UtlOfVariable;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable]
    public partial class CalcVector3dListFuncPar : FunctionFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.calcVector3dList;
        public CalcVector3dOperatorType operatorType;
        public VariableDataVector3DList parameter1Vv3 = new();
        public VariableDataVector3Get parameter2Vv3 = new();
        public VariableDataNumericGet parameter2Vn = new();
        public VariableDataVector3DList targetVariableV = new();
        public VariableDataNumericList targetVariableN = new();
        public ListCalcType listCalcType;

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (CalcVector3dOperatorType* opt = &operatorType)
            fixed (ListCalcType* lct = &listCalcType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_calcVector3dListFuncPar.operatorType, pgNodeParDescription_calcVector3dListFuncPar.operatorType);
                pgbepManager.SetPgbepEnum(typeof(CalcVector3dOperatorType), (int*)opt);

                pgbepManager.SetHeaderText(pgNodeParameter_calcVector3dListFuncPar.parameter1, pgNodeParDescription_calcVector3dListFuncPar.parameter1);
                pgbepManager.SetPgbepVariable(parameter1Vv3, false);

                GetUseVariableFlags(out var indicateP2Vv3, out var indicateP2Vn);
                if (indicateP2Vv3 || indicateP2Vn)
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_calcVector3dListFuncPar.parameter2, pgNodeParDescription_calcVector3dListFuncPar.parameter2);
                    pgbepManager.SetPgbepEnum(typeof(ListCalcType), (int*)lct);
                }
                if (indicateP2Vv3)
                {
                    switch (listCalcType)
                    {
                        case ListCalcType.Element:
                            parameter2Vv3.IndicateSwitchable(pgbepManager, null, parameter2Vv3.selectableVariableTypes);
                            break;
                        case ListCalcType.List:
                            pgbepManager.SetPgbepVariable(targetVariableV, false, new List<VariableType> { VariableType.Vector3DList });
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else if (indicateP2Vn)
                {
                    switch (listCalcType)
                    {
                        case ListCalcType.Element:
                            parameter2Vn.IndicateSwitchableFloat(pgbepManager, null, parameter2Vn.selectableVariableTypes);
                            break;
                        case ListCalcType.List:
                            pgbepManager.SetPgbepVariable(targetVariableN, false, new List<VariableType> { VariableType.NumericList });
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                GetUseTgtVariableFlags(out var useTgtV, out var useTgtN);
                pgbepManager.SetHeaderText(pgNodeParameter_calcVector3dListFuncPar.targetVariable, pgNodeParDescription_calcVector3dListFuncPar.targetVariable);
                targetVariableV.useVariable = useTgtV;
                targetVariableN.useVariable = useTgtN;
                if (useTgtV) pgbepManager.SetPgbepVariable(targetVariableV, false);
                else if (useTgtN) pgbepManager.SetPgbepVariable(targetVariableN, false);
            }
        }

        private List<float> _resNList = new();
        private List<Vector3> _resVList = new();
        public void ExeCalcVector3dList(MachineLD ld)
        {
            var p1L = parameter1Vv3.GetUseValue(ld);
            _resNList.Clear();
            _resVList.Clear();
            GetUseVariableFlags(out var indicateP2Vv3, out var indicateP2Vn);
            switch (indicateP2Vn, indicateP2Vv3, listCalcType)
            {
                case (false, false, _):
                    foreach (var p1 in p1L)
                    {
                        var fr = CalcVector3d(ld, operatorType, p1, null, null);
                        if (fr.resN.HasValue) _resNList.Add(fr.resN.Value);
                        if (fr.resV.HasValue) _resVList.Add(fr.resV.Value);
                    }
                    break;
                case (true, false, ListCalcType.Element):
                    var p2N = parameter2Vn.GetUseValueFloat(ld, null, null);
                    foreach (var p1 in p1L)
                    {
                        var fr = CalcVector3d(ld, operatorType, p1, null, p2N);
                        if (fr.resN.HasValue) _resNList.Add(fr.resN.Value);
                        if (fr.resV.HasValue) _resVList.Add(fr.resV.Value);
                    }
                    break;
                case (false, true, ListCalcType.Element):
                    var p2V = parameter2Vv3.GetUseValue(ld, null, null);
                    foreach (var p1 in p1L)
                    {
                        var fr = CalcVector3d(ld, operatorType, p1, p2V, null);
                        if (fr.resN.HasValue) _resNList.Add(fr.resN.Value);
                        if (fr.resV.HasValue) _resVList.Add(fr.resV.Value);
                    }
                    break;
                case (true, false, ListCalcType.List):
                    var p2Nl = parameter2Vn.GetUseList(ld);
                    for (var i = 0; i < p1L.Count || i < p2Nl.Count; i++)
                    {
                        var p1 = i >= 0 && i < p1L.Count ? p1L[i] : Vector3.zero;
                        var fr = CalcVector3d(ld, operatorType, p1, null, i >= 0 && i < p2Nl.Count ? p2Nl[i] : 0);
                        if (!fr.resN.HasValue && !fr.resV.HasValue) break;
                        if (fr.resN.HasValue) _resNList.Add(fr.resN.Value);
                        if (fr.resV.HasValue) _resVList.Add(fr.resV.Value);
                    }
                    break;
                case (false, true, ListCalcType.List):
                    var p2Vl = parameter2Vv3.GetUseList(ld);
                    for (var i = 0; i < p1L.Count || i < p2Vl.Count; i++)
                    {
                        var p1 = i >= 0 && i < p1L.Count ? p1L[i] : Vector3.zero;
                        var fr = CalcVector3d(ld, operatorType, p1, i >= 0 && i < p2Vl.Count ? p2Vl[i] : Vector3.zero, null);
                        if (!fr.resN.HasValue && !fr.resV.HasValue) break;
                        if (fr.resN.HasValue) _resNList.Add(fr.resN.Value);
                        if (fr.resV.HasValue) _resVList.Add(fr.resV.Value);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GetUseTgtVariableFlags(out var useTgtV, out var useTgtN);
            if (useTgtN)
            {
                targetVariableN.SetValue(ld, _resNList);
            }
            if (useTgtV)
            {
                targetVariableV.SetValue(ld, _resVList);
            }
        }

        public void GetUseVariableFlags(out bool useP2Vv3, out bool useP2Vn)
        {
            useP2Vv3 = false;
            useP2Vn = false;
            switch (operatorType)
            {
                case CalcVector3dOperatorType.Assignment:
                case CalcVector3dOperatorType.Normalize:
                case CalcVector3dOperatorType.Magnitude:
                case CalcVector3dOperatorType.ConvertPointGlobalToLocal:
                case CalcVector3dOperatorType.ConvertDirectionGlobalToLocal:
                case CalcVector3dOperatorType.ConvertVectorGlobalToLocal:
                case CalcVector3dOperatorType.ConvertPointLocalToGlobal:
                case CalcVector3dOperatorType.ConvertDirectionLocalToGlobal:
                case CalcVector3dOperatorType.ConvertVectorLocalToGlobal:
                    break;
                case CalcVector3dOperatorType.Addition:
                case CalcVector3dOperatorType.Subtraction:
                case CalcVector3dOperatorType.MultiplicationVector3D:
                case CalcVector3dOperatorType.Max:
                case CalcVector3dOperatorType.Min:
                case CalcVector3dOperatorType.Angle:
                case CalcVector3dOperatorType.Distance:
                case CalcVector3dOperatorType.Project:
                case CalcVector3dOperatorType.ProjectOnPlane:
                case CalcVector3dOperatorType.Cross:
                case CalcVector3dOperatorType.Dot:
                    useP2Vv3 = true;
                    break;
                case CalcVector3dOperatorType.MultiplicationNumeric:
                case CalcVector3dOperatorType.Division:
                    useP2Vn = true;
                    break;
            }
        }
        public void GetUseTgtVariableFlags(out bool useTgtV, out bool useTgtN)
        {
            useTgtV = false;
            useTgtN = false;
            switch (operatorType)
            {
                case CalcVector3dOperatorType.Assignment:
                case CalcVector3dOperatorType.Addition:
                case CalcVector3dOperatorType.Subtraction:
                case CalcVector3dOperatorType.MultiplicationNumeric:
                case CalcVector3dOperatorType.MultiplicationVector3D:
                case CalcVector3dOperatorType.Division:
                case CalcVector3dOperatorType.Max:
                case CalcVector3dOperatorType.Min:
                case CalcVector3dOperatorType.Normalize:
                case CalcVector3dOperatorType.Project:
                case CalcVector3dOperatorType.ProjectOnPlane:
                case CalcVector3dOperatorType.Cross:
                case CalcVector3dOperatorType.ConvertPointGlobalToLocal:
                case CalcVector3dOperatorType.ConvertDirectionGlobalToLocal:
                case CalcVector3dOperatorType.ConvertVectorGlobalToLocal:
                case CalcVector3dOperatorType.ConvertPointLocalToGlobal:
                case CalcVector3dOperatorType.ConvertDirectionLocalToGlobal:
                case CalcVector3dOperatorType.ConvertVectorLocalToGlobal:
                    useTgtV = true;
                    break;
                case CalcVector3dOperatorType.Magnitude:
                case CalcVector3dOperatorType.Angle:
                case CalcVector3dOperatorType.Distance:
                case CalcVector3dOperatorType.Dot:
                    useTgtN = true;
                    break;
            }
        }

        public override string[] GetNodeFaceText()
        {
            var operatorStr = operatorType switch
            {
                CalcVector3dOperatorType.Assignment => "->",
                CalcVector3dOperatorType.Addition => "+",
                CalcVector3dOperatorType.Subtraction => "-",
                CalcVector3dOperatorType.MultiplicationNumeric => "×(N)",
                CalcVector3dOperatorType.MultiplicationVector3D => "×(V)",
                CalcVector3dOperatorType.Division => "÷",
                CalcVector3dOperatorType.Max => "Max",
                CalcVector3dOperatorType.Min => "Min",
                CalcVector3dOperatorType.Normalize => "Normalize",
                CalcVector3dOperatorType.Magnitude => "Magnitude",
                CalcVector3dOperatorType.Angle => "Angle",
                CalcVector3dOperatorType.Distance => "Distance",
                CalcVector3dOperatorType.Project => "Project",
                CalcVector3dOperatorType.ProjectOnPlane => "ProjectOnPlane",
                CalcVector3dOperatorType.Cross => "Cross",
                CalcVector3dOperatorType.Dot => "Dot",
                CalcVector3dOperatorType.ConvertPointGlobalToLocal => "Point G->L",
                CalcVector3dOperatorType.ConvertDirectionGlobalToLocal => "Direction G->L",
                CalcVector3dOperatorType.ConvertVectorGlobalToLocal => "Vector G->L",
                CalcVector3dOperatorType.ConvertPointLocalToGlobal => "Point L->G",
                CalcVector3dOperatorType.ConvertDirectionLocalToGlobal => "Direction L->G",
                CalcVector3dOperatorType.ConvertVectorLocalToGlobal => "Vector L->G",
                _ => throw new ArgumentOutOfRangeException()
            };
            GetUseVariableFlags(out var useP2Vv3, out var useP2Vn);
            GetUseTgtVariableFlags(out var useTgtV, out var useTgtN);
            var tgtStr = useTgtV ? targetVariableV.GetIndicateStr() : targetVariableN.GetIndicateStr();
            if (useP2Vv3 || useP2Vn)
            {
                var p2Str = useP2Vv3 ? parameter2Vv3.GetIndicateStr(null, null, listCalcType) : parameter2Vn.GetIndicateStr(null, 1, listCalcType);
                return new[]
                {
                    $"{parameter1Vv3.GetIndicateStr()}\n{operatorStr}\n{p2Str}\n->\n{tgtStr}"
                };
            }
            else
            {
                return new[] { $"{parameter1Vv3.GetIndicateStr()}\n{operatorStr}\n{tgtStr}" };
            }
        }
    }
}