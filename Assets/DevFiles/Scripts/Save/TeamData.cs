using clrev01.ClAction.Machines;
using MemoryPack;
using System.Collections.Generic;

namespace clrev01.Save
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class TeamData : SaveData
    {
        public List<CustomData> machineList = new() { };
        public List<int> machinePositions = new();
    }
}