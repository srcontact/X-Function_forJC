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
    public partial class VariableDataNumeric : VariableData
    {
        public VariableDataIndex indexV = new();
        public override List<VariableType> selectableVariableTypes { get; } = new() { VariableType.Numeric, VariableType.NumericList };
        [MemoryPackIgnore]
        [NonSerialized]
        protected IVariableValueNumeric value;

        public VariableDataNumeric()
        {
            name = "Numeric01";
            variableType = VariableType.Numeric;
        }
    }

    [MemoryPackable]
    public partial class VariableDataNumericGet : VariableDataNumeric
    {
        public float constValue;

        public unsafe void IndicateSwitchableFloat(PgbepManager pgbepManager, PgbepManager.FloatSliderSettingPar sliderSetting = null, List<VariableType> selectableVariableOverwrite = null)
        {
            if (useVariable)
            {
                pgbepManager.SetPgbepVariable(this, true, selectableVariableOverwrite);
                if (variableType is VariableType.NumericList) indexV.IndicateSwitchable(pgbepManager);
            }
            else
            {
                fixed (float* cv = &constValue) pgbepManager.SetPgbepFloat(cv, sliderSetting, this, selectableVariableOverwrite);
            }
        }
        public unsafe void IndicateSwitchableInt(PgbepManager pgbepManager, PgbepManager.IntSliderSettingPar sliderSetting = null, List<VariableType> selectableVariableTypes = null)
        {
            if (useVariable)
            {
                pgbepManager.SetPgbepVariable(this, true, selectableVariableTypes);
                if (variableType is VariableType.NumericList) indexV.IndicateSwitchable(pgbepManager);
            }
            else
            {
                fixed (float* cv = &constValue)
                {
                    pgbepManager.SetPgbepInt(cv, sliderSetting, this, selectableVariableTypes);
                }
            }
        }
        public float GetUseValueFloat([NotNull] MachineLD ld, float? min = null, float? max = null)
        {
            float res;
            if (!useVariable) res = constValue;
            else
            {
                value ??= ld.RegisterVariableDict<IVariableValueNumeric>(this);
                res = value.GetNumericValue(indexV.GetUseValue(ld));
            }
            if (min.HasValue) res = Mathf.Max(res, min.Value);
            if (max.HasValue) res = Mathf.Min(res, max.Value);
            return res;
        }
        public int GetUseValueInt([NotNull] MachineLD ld, int? min = null, int? max = null)
        {
            int res;
            if (!useVariable) res = (int)constValue;
            else
            {
                value ??= ld.RegisterVariableDict<IVariableValueNumeric>(this);
                res = (int)value.GetNumericValue(indexV.GetUseValue(ld));
            }
            if (min.HasValue) res = Mathf.Max(res, min.Value);
            if (max.HasValue) res = Mathf.Min(res, max.Value);
            return res;
        }
        public List<float> GetUseList([NotNull] MachineLD ld)
        {
            value ??= ld.RegisterVariableDict<VariableValueNumericList>(this);
            return (value as VariableValueNumericList)?.Value;
        }
        public string GetIndicateStr(string unit = null, float ratio = 1, ListCalcType listCalcType = ListCalcType.Element)
        {
            if (useVariable) return $"[{UtlOfCL.GetEllipsisString(name, 16, 5)}{(variableType is VariableType.NumericList && listCalcType is ListCalcType.Element ? indexV.GetIndexStr() : null)}]{unit}";
            return $"<nobr>{(constValue * ratio).ToString($"0.###{unit}")}</nobr>";
        }
        public float? GetGaugeValue(float ratio = 1, bool abs = false)
        {
            if (useVariable) return null;
            var gaugeValue = constValue * ratio;
            if (abs) return Mathf.Abs(gaugeValue);
            return gaugeValue;
        }
    }

    [MemoryPackable]
    public partial class VariableDataNumericSet : VariableDataNumeric
    {
        public AddToListMethodType addToListMethodType = AddToListMethodType.Replace;

        public unsafe void IndicateSwitchable(PgbepManager pgbepManager, List<VariableType> selectableVariableOverwrite = null)
        {
            pgbepManager.SetPgbepVariable(this, true, selectableVariableOverwrite);
            if (variableType is VariableType.NumericList)
            {
                fixed (AddToListMethodType* atl = &addToListMethodType)
                {
                    pgbepManager.SetPgbepEnum(typeof(AddToListMethodType), (int*)atl);
                    if (addToListMethodType is not AddToListMethodType.Add and not AddToListMethodType.UniqueAdd) indexV.IndicateSwitchable(pgbepManager);
                }
            }
        }
        public void SetNumericValue(MachineLD ld, float v)
        {
            value ??= ld.RegisterVariableDict<IVariableValueNumeric>(this);
            value.SetNumericValue(v, indexV.GetUseValue(ld), addToListMethodType);
        }
        public void SetNumericList(MachineLD ld, params float[] vl)
        {
            value ??= ld.RegisterVariableDict<VariableValueNumericList>(this);
            var list = (value as VariableValueNumericList)?.Value;
            list?.Clear();
            list?.AddRange(vl);
        }
        public string GetIndicateStr(string unit = null, ListCalcType listCalcType = ListCalcType.Element)
        {
            return useVariable ? $"[{UtlOfCL.GetEllipsisString(name, 16, 5)}{(variableType is VariableType.NumericList && listCalcType is ListCalcType.Element ? indexV.GetIndexStr() : null)}]{unit}" : null;
        }
    }
}