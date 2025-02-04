using System;
using UnityEngine;

namespace clrev01.Programs.FieldPar
{
    [Serializable]
    public class BoxSearchFieldParForInspector : BoxSearchFieldPar
    {
        [SerializeField]
        private Vector3 size;
        [SerializeField]
        private Vector3 offset;
        [SerializeField]
        private Vector3 rotate;
        [SerializeField]
        private float rotate2d;
        [SerializeField]
        private bool is2d;

        public override Vector3 Size => size;
        public override Vector3 Offset => offset;
        public override Vector3 Rotate => rotate;
        public override float Rotate2d => rotate2d;
        public override bool Is2D => is2d;
    }
}