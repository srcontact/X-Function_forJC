using MemoryPack;

namespace clrev01.Save
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class SaveData
    {
        [MemoryPackIgnore]
        public string fileName = "Untitled";
        public string dataName = "Untitled";

        public virtual void InitializeData()
        { }
    }
}