using MemoryPack;
using UnityEngine;

namespace clrev01.Programs
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class PGBEditorPar
    {
        public int myIndex;
        public int nextIndex;
        public int falseNextIndex;
        [SerializeField]
        private Vector2 editorPos;
        public Vector2 EditorPos
        {
            get => editorPos;
            set => editorPos = EditorPosRound(value);
        }
        /// <summary>
        /// どのルーチンに属しているかを示す番号
        /// </summary>
        [MemoryPackIgnore]
        public int routineNum { get; set; }


        [MemoryPackConstructor]
        public PGBEditorPar() { }
        public PGBEditorPar(int myIndex, int nextIndex, int falseNextIndex, Vector2 editorPos)
        {
            this.myIndex = myIndex;
            this.nextIndex = nextIndex;
            this.falseNextIndex = falseNextIndex;
            this.editorPos = editorPos;
            this.editorPos = EditorPosRound(editorPos);
        }

        private Vector2 EditorPosRound(Vector2 vector2)
        {
            vector2.x = Mathf.RoundToInt(vector2.x / 50f) * 50;
            vector2.y = Mathf.RoundToInt(vector2.y / 50f) * 50;
            return vector2;
        }
    }
}