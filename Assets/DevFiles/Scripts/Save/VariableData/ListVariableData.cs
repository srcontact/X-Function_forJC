using clrev01.Bases;
using MemoryPack;

namespace clrev01.Save.VariableData
{
    [MemoryPackable()]
    [MemoryPackUnion(0, typeof(VariableDataNumericList))]
    [MemoryPackUnion(1, typeof(VariableDataVector3DList))]
    [MemoryPackUnion(2, typeof(VariableDataLockOnList))]
    public abstract partial class ListVariableData : VariableData
    {
        public string GetIndicateStr(string targetNumber = null)
        {
            return $"[{UtlOfCL.GetEllipsisString(name, 16, 5)}{(targetNumber is not null ? $"[{targetNumber}]" : "")}]";
        }
    }
}