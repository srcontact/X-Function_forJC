namespace clrev01.Save.DataManageObj
{
    [System.Serializable]
    public class FileManageObjTD : FileManageObj<TeamData>
    {
        public override string fileExt => ".xft";
        public FileManageObjTD(string root) : base(root) { }
    }
}