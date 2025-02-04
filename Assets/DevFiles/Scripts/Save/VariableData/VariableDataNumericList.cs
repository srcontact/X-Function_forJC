using clrev01.ClAction.Machines;
using clrev01.PGE.VariableEditor;
using clrev01.Programs;
using JetBrains.Annotations;
using MemoryPack;
using System;
using System.Collections.Generic;

namespace clrev01.Save.VariableData
{
    [Serializable]
    [MemoryPackable()]
    public partial class VariableDataNumericList : ListVariableData
    {
        public override List<VariableType> selectableVariableTypes { get; } = new() { VariableType.NumericList };
        [MemoryPackIgnore]
        [NonSerialized]
        private VariableValueNumericList _value;


        public VariableDataNumericList()
        {
            name = "NumericList01";
            variableType = VariableType.NumericList;
        }


        public List<float> GetUseValue([NotNull] MachineLD ld)
        {
            _value ??= ld.RegisterVariableDict<VariableValueNumericList>(this);
            return _value.Value;
        }
        public void SetValue(MachineLD ld, IEnumerable<float> vl)
        {
            _value ??= ld.RegisterVariableDict<VariableValueNumericList>(this);
            _value.Value.Clear();
            _value.Value.AddRange(vl);
        }
    }
}