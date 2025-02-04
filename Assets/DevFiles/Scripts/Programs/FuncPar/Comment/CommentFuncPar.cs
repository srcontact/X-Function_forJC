using clrev01.PGE.PGBEditor;
using clrev01.Programs.FieldPar;
using clrev01.Programs.FuncPar.FuncParType;
using MemoryPack;

namespace clrev01.Programs.FuncPar.Comment
{
    [MemoryPackable()]
    public partial class CommentFuncPar : IPGBFuncUnion
    {
        public string BlockTypeStr => "Comment";
        public int GetFuncParNum { get; }
        public int calcCost => 0;
        public bool OpenEditorOnCreateNode => true;
        public StringRefObj commentText = new();
        public void SetPointers(PgbepManager pgbepManager)
        {
            pgbepManager.SetPgbepComment(commentText, () => { });
        }
        public bool IsConnectable(IPGBFuncUnion connectionFrom)
        {
            return connectionFrom is CommentFuncPar;
        }
        public string[] GetNodeFaceText()
        {
            return new[] { commentText.obj };
        }
        public float?[] GetNodeFaceValue()
        {
            return null;
        }
        public StopActionType? GetNodeFaceStopActionType()
        {
            return null;
        }
        public long? GetNodeFaceSelectedWeapons()
        {
            return null;
        }
        public int? GetNodeFaceWeaponIcon()
        {
            return null;
        }
        public IFieldEditObject GetNodeFaceIFieldEditObject()
        {
            return null;
        }
    }
}