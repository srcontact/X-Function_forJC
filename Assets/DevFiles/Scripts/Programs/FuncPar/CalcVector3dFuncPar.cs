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
    [MemoryPackable()]
    public partial class CalcVector3dFuncPar : FunctionFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.calcVector3d;
        public CalcVector3dOperatorType operatorType;
        public VariableDataVector3Get parameter1Vv3 = new();
        public VariableDataVector3Get parameter2Vv3 = new();
        public VariableDataNumericGet parameter2Vn = new();
        public VariableDataVector3Set targetVariableV = new();
        public VariableDataNumericSet targetVariableN = new();


        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (CalcVector3dOperatorType* opt = &operatorType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_calcVector3dFuncPar.operatorType, pgNodeParDescription_calcVector3dFuncPar.operatorType);
                pgbepManager.SetPgbepEnum(typeof(CalcVector3dOperatorType), (int*)opt);

                var vector3dVariableTypes = new List<VariableType> { VariableType.Vector3D, VariableType.Vector3DList };
                var numericVariableTypes = new List<VariableType> { VariableType.Numeric, VariableType.NumericList };

                pgbepManager.SetHeaderText(pgNodeParameter_calcVector3dFuncPar.parameter1, pgNodeParDescription_calcVector3dFuncPar.parameter1);
                parameter1Vv3.IndicateSwitchable(pgbepManager, null, vector3dVariableTypes);

                GetUseVariableFlags(out var indicateP2Vv3, out var indicateP2Vn);
                if (indicateP2Vv3)
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_calcVector3dFuncPar.parameter2, pgNodeParDescription_calcVector3dFuncPar.parameter2);
                    parameter2Vv3.IndicateSwitchable(pgbepManager, null, vector3dVariableTypes);
                }
                else if (indicateP2Vn)
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_calcVector3dFuncPar.parameter2, pgNodeParDescription_calcVector3dFuncPar.parameter2);
                    parameter2Vn.IndicateSwitchableFloat(pgbepManager, null, numericVariableTypes);
                }

                GetUseTgtVariableFlags(out var useTgtV, out var useTgtN);
                pgbepManager.SetHeaderText(pgNodeParameter_calcVector3dFuncPar.targetVariable, pgNodeParDescription_calcVector3dFuncPar.targetVariable);
                targetVariableV.useVariable = useTgtV;
                targetVariableN.useVariable = useTgtN;
                if (useTgtV) targetVariableV.IndicateSwitchable(pgbepManager);
                else if (useTgtN) targetVariableN.IndicateSwitchable(pgbepManager);
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

        public void ExeCalcVector3d(MachineLD ld)
        {
            var pv1 = parameter1Vv3.GetUseValue(ld);
            var pv2 = parameter2Vv3.GetUseValue(ld);
            var pn2 = parameter2Vn.GetUseValueFloat(ld);
            var res = CalcVector3d(ld, operatorType, pv1, pv2, pn2);
            if (res.resV.HasValue) targetVariableV.SetVector3dValue(ld, res.resV.Value);
            if (res.resN.HasValue) targetVariableN.SetNumericValue(ld, res.resN.Value);
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
                return new[]
                {
                    $"{parameter1Vv3.GetIndicateStr(null, null)}\n{operatorStr}\n{(useP2Vv3 ? parameter2Vv3.GetIndicateStr(null, null) : parameter2Vn.GetIndicateStr(null))}\n->\n{tgtStr}"
                };
            }
            else
            {
                return new[] { $"{parameter1Vv3.GetIndicateStr(null, null)}\n{operatorStr}\n{tgtStr}" };
            }
        }
    }
}