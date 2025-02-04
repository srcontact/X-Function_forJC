using clrev01.ClAction.Machines;

namespace clrev01.Save.DataManageObj
{
    [System.Serializable]
    public class FileManageObjCD : FileManageObj<CustomData>
    {
        public override string fileExt => ".xfc";
        public FileManageObjCD(string root) : base(root) { }
    }
}