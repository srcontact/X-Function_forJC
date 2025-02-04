using clrev01.ClAction;
using clrev01.Extensions;
using static clrev01.Extensions.ExUtls;

namespace clrev01.Bases
{
    [System.Serializable]
    public abstract class LocalDataBase : NestedClassBase
    {
        private static int _nextUniqueID = 0;
        public int uniqueID;
        public int spawnFrame = 0;
        public int ExeFrameCount => ActionManager.Inst.actionFrame - spawnFrame;
        public string tag;
        public abstract HardBase hdb
        {
            get;
        }
        public abstract CommonDataBase cdb
        {
            get;
        }

        public static void ResetUniqueId()
        {
            _nextUniqueID = 0;
        }

        public void SetUniqueID()
        {
            uniqueID = _nextUniqueID;
            _nextUniqueID++;
        }

        public virtual void RegisterTag(string nTag)
        {
            tag = nTag;
            hdb.tag = nTag;
        }
    }
}