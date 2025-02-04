using clrev01.PGE.VariableEditor;
using MemoryPack;
using System.Collections.Generic;

namespace clrev01.Save.VariableData
{
    [MemoryPackable()]
    [MemoryPackUnion(0, typeof(VariableDataNumeric))]
    [MemoryPackUnion(1, typeof(VariableDataNumericGet))]
    [MemoryPackUnion(2, typeof(VariableDataNumericSet))]
    [MemoryPackUnion(3, typeof(VariableDataVector3))]
    [MemoryPackUnion(4, typeof(VariableDataVector3Get))]
    [MemoryPackUnion(5, typeof(VariableDataVector3Set))]
    [MemoryPackUnion(6, typeof(VariableDataLockOn))]
    [MemoryPackUnion(7, typeof(VariableDataLockOnGet))]
    [MemoryPackUnion(8, typeof(VariableDataLockOnSet))]
    [MemoryPackUnion(9, typeof(ListVariableData))]
    [MemoryPackUnion(10, typeof(VariableDataNumericList))]
    [MemoryPackUnion(11, typeof(VariableDataVector3DList))]
    [MemoryPackUnion(12, typeof(VariableDataLockOnList))]
    [MemoryPackUnion(13, typeof(VariableDataIndex))]
    public abstract partial class VariableData
    {
        public string name;
        public bool useVariable;
        public VariableType variableType;
        [MemoryPackIgnore]
        public PGBData ownerDara { get; set; }
        [MemoryPackIgnore]
        public abstract List<VariableType> selectableVariableTypes { get; }
    }
}