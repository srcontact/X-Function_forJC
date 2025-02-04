using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.PGE.VariableEditor;
using clrev01.Programs;
using clrev01.Programs.FuncPar;
using JetBrains.Annotations;
using MemoryPack;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace clrev01.Save.VariableData
{
    [Serializable]
    [MemoryPackable()]
    public partial class VariableDataVector3 : VariableData
    {
        public VariableDataIndex indexV = new();
        public override List<VariableType> selectableVariableTypes { get; } = new() { VariableType.Vector3D, VariableType.Vector3DList };

        [MemoryPackIgnore]
        [NonSerialized]
        protected IVariableValueVector3D value;

        public VariableDataVector3()
        {
            name = "Vector3d01";
            variableType = VariableType.Vector3D;
        }
    }

    [MemoryPackable]
    public partial class VariableDataVector3Get : VariableDataVector3
    {
        public Vector3 constValue;

        public unsafe void IndicateSwitchable(PgbepManager pgbepManager, IReadOnlyList<bool> activeList = null, List<VariableType> selectableVariableOverwrite = null)
        {
            if (useVariable)
            {
                pgbepManager.SetPgbepVariable(this, true, selectableVariableOverwrite);
                if (variableType is VariableType.Vector3DList) indexV.IndicateSwitchable(pgbepManager);
            }
            else
            {
                fixed (Vector3* cv = &constValue) pgbepManager.SetPgbepVector3(cv, activeList, this, selectableVariableOverwrite);
            }
        }

        public unsafe void IndicateSwitchableForFieldEditor(PgbepManager pgbepManager, string headerStr, PgbepManager.FloatSliderSettingPar sliderSettingPar = null, IReadOnlyList<bool> activeList = null, List<VariableType> selectableVariableOverwrite = null)
        {
            if (useVariable)
            {
                pgbepManager.SetHeaderText(headerStr);
                pgbepManager.SetPgbepVariable(this, true, selectableVariableOverwrite);
                if (variableType is VariableType.Vector3DList) indexV.IndicateSwitchable(pgbepManager);
            }
            else
            {
                if (activeList == null || (activeList.Count > 0 && activeList[0]))
                {
                    pgbepManager.SetHeaderText($"{headerStr}:X");
                    fixed (float* x = &constValue.x) pgbepManager.SetPgbepFloat(x, sliderSettingPar, this);
                }
                if (activeList == null || (activeList.Count > 1 && activeList[1]))
                {
                    pgbepManager.SetHeaderText($"{headerStr}:Y");
                    fixed (float* y = &constValue.y) pgbepManager.SetPgbepFloat(y, sliderSettingPar, this);
                }
                if (activeList == null || (activeList.Count > 2 && activeList[2]))
                {
                    pgbepManager.SetHeaderText($"{headerStr}:Z");
                    fixed (float* z = &constValue.z) pgbepManager.SetPgbepFloat(z, sliderSettingPar, this);
                }
            }
        }
        public Vector3 GetUseValue([NotNull] MachineLD ld, Vector3? min = null, Vector3? max = null)
        {
            Vector3 res;
            if (!useVariable) res = constValue;
            else
            {
                value ??= ld.RegisterVariableDict<IVariableValueVector3D>(this);
                res = value.GetVector3DValue(indexV.GetUseValue(ld));
            }
            if (min.HasValue) res = Vector3.Max(res, min.Value);
            if (max.HasValue) res = Vector3.Min(res, max.Value);
            return res;
        }
        public List<Vector3> GetUseList([NotNull] MachineLD ld)
        {
            value ??= ld.RegisterVariableDict<VariableValueVector3DList>(this);
            return (value as VariableValueVector3DList)?.Value;
        }
        public string GetIndicateStr(bool[] ignoreList = null, string unit = null, ListCalcType listCalcType = ListCalcType.Element)
        {
            if (useVariable)
            {
                return $"[{UtlOfCL.GetEllipsisString(name, 16, 5)}{(variableType is VariableType.Vector3DList && listCalcType is ListCalcType.Element ? indexV.GetIndexStr() : null)}]";
            }
            var res = "(";
            var addComma = false;
            for (var i = 0; i < 3; i++)
            {
                if (ignoreList != null && i < ignoreList.Length && ignoreList[i]) continue;
                if (addComma) res += ",";
                res += $"<nobr>{constValue[i].ToString($"0.###")}</nobr>";
                addComma = true;
            }
            res += $"){unit}";
            return res;
        }
    }

    [MemoryPackable]
    public partial class VariableDataVector3Set : VariableDataVector3
    {
        public AddToListMethodType addToListMethodType = AddToListMethodType.Replace;

        public unsafe void IndicateSwitchable(PgbepManager pgbepManager, List<VariableType> selectableVariableOverwrite = null)
        {
            pgbepManager.SetPgbepVariable(this, true, selectableVariableOverwrite);
            if (variableType is VariableType.Vector3DList)
            {
                fixed (AddToListMethodType* atl = &addToListMethodType)
                {
                    pgbepManager.SetPgbepEnum(typeof(AddToListMethodType), (int*)atl);
                    if (addToListMethodType is not AddToListMethodType.Add and not AddToListMethodType.UniqueAdd) indexV.IndicateSwitchable(pgbepManager);
                }
            }
        }
        public void SetVector3dValue(MachineLD ld, Vector3 v)
        {
            value ??= ld.RegisterVariableDict<IVariableValueVector3D>(this);
            value.SetVector3DValue(v, indexV.GetUseValue(ld), addToListMethodType);
        }
        public void SetVector3dList(MachineLD ld, params Vector3[] vl)
        {
            value ??= ld.RegisterVariableDict<VariableValueVector3DList>(this);
            var list = (value as VariableValueVector3DList)?.Value;
            list?.Clear();
            list?.AddRange(vl);
        }
        public string GetIndicateStr(string unit = null, ListCalcType listCalcType = ListCalcType.Element)
        {
            return useVariable ? $"[{UtlOfCL.GetEllipsisString(name, 16, 5)}{(variableType is VariableType.Vector3DList && listCalcType is ListCalcType.Element ? indexV.GetIndexStr() : null)}]{unit}" : null;
        }
    }
}