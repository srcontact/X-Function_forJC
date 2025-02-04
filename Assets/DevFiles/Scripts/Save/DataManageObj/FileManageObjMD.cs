namespace clrev01.Save.DataManageObj
{
    [System.Serializable]
    public class FileManageObjMD : FileManageObj<MatchData>
    {
        public override string fileExt => ".xfm";
        public FileManageObjMD(string root) : base(root)
        { }
    }
}