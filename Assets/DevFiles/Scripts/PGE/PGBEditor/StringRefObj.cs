using MemoryPack;

namespace clrev01.PGE.PGBEditor
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class StringRefObj
    {
        public string obj;

        public StringRefObj(string obj = "")
        {
            this.obj = obj;
        }
    }
}