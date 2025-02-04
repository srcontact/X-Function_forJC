using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.PGE.VariableEditor;
using clrev01.Programs;
using MemoryPack;
using System.Collections.Generic;

namespace clrev01.Save.VariableData
{
    [MemoryPackable]
    public partial class VariableDataIndex : VariableData
    {
        public override List<VariableType> selectableVariableTypes => new() { VariableType.Numeric };
        public List<VariableType> _selectableVariableTypes => new() { VariableType.Numeric };
        public float constValue;
        [MemoryPackIgnore]
        public VariableValueNumeric value;

        public unsafe void IndicateSwitchable(PgbepManager pgbepManager)
        {
            if (useVariable)
            {
                pgbepManager.SetPgbepVariable(this, true, selectableVariableTypes);
            }
            else
            {
                fixed (float* cv = &constValue) pgbepManager.SetPgbepInt(cv, null, this, selectableVariableTypes);
            }
        }

        public int GetUseValue(MachineLD ld)
        {
            if (!useVariable) return (int)constValue;
            value ??= ld.RegisterVariableDict<VariableValueNumeric>(this);
            return (int)value.GetNumericValue(0);
        }

        public string GetIndexStr()
        {
            var indexStr = $"[{(useVariable ? name : constValue.ToString())}]";
            return indexStr;
        }
    }
}