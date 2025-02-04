using MemoryPack;
using UnityEngine;

namespace clrev01.Save
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class CommentNodeData
    {
        public int myIndex;
        public int connectIndex;
        public Vector2 editorPos;
        public string commentText;
    }
}