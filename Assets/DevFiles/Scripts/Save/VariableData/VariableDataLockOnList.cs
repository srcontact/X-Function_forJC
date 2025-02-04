using clrev01.ClAction.Machines;
using clrev01.ClAction.ObjectSearch;
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
    public partial class VariableDataLockOnList : ListVariableData
    {
        public override List<VariableType> selectableVariableTypes { get; } = new() { VariableType.LockOnList };
        [MemoryPackIgnore]
        [NonSerialized]
        private VariableValueLockOnList _value;


        public VariableDataLockOnList()
        {
            name = "LockOnList01";
            variableType = VariableType.LockOnList;
        }

        public List<ObjectSearchTgt> GetUseValue([NotNull] MachineLD ld)
        {
            _value ??= ld.RegisterVariableDict<VariableValueLockOnList>(this);
            return _value.Value;
        }

        public void RemoveNull(MachineLD ld)
        {
            _value ??= ld.RegisterVariableDict<VariableValueLockOnList>(this);
            for (var i = _value.Value.Count - 1; i >= 0; i--)
            {
                if (_value.Value[i] is null) _value.Value.RemoveAt(i);
            }
        }
    }
}