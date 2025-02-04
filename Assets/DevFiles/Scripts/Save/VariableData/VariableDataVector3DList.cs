using clrev01.ClAction.Machines;
using clrev01.PGE.VariableEditor;
using clrev01.Programs;
using MemoryPack;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace clrev01.Save.VariableData
{
    [Serializable]
    [MemoryPackable()]
    public partial class VariableDataVector3DList : ListVariableData
    {
        public override List<VariableType> selectableVariableTypes { get; } = new() { VariableType.Vector3DList };
        [MemoryPackIgnore]
        [NonSerialized]
        private VariableValueVector3DList _value;


        public VariableDataVector3DList()
        {
            name = "Vector3dList01";
            variableType = VariableType.Vector3DList;
        }

        public List<Vector3> GetUseValue(MachineLD ld)
        {
            _value ??= ld.RegisterVariableDict<VariableValueVector3DList>(this);
            return _value.Value;
        }
        public void SetValue(MachineLD ld, IEnumerable<Vector3> vl)
        {
            _value ??= ld.RegisterVariableDict<VariableValueVector3DList>(this);
            _value.Value.Clear();
            _value.Value.AddRange(vl);
        }
    }
}