using System;
using UnityEngine;

namespace clrev01.Programs.FieldPar
{
    [Serializable]
    public class CircleSearchFieldParForInspector : CircleSearchFieldPar
    {
        [SerializeField]
        private float farRadius;
        [SerializeField]
        private float nearRadius;
        [SerializeField]
        private float angle;
        [SerializeField]
        private float rotate;
        [SerializeField]
        private float height;
        [SerializeField]
        private Vector3 offset;
        [SerializeField]
        private bool is2d;

        public override float FarRadius => farRadius;
        public override float NearRadius => nearRadius;
        public override float Angle => angle;
        public override float Rotate => rotate;
        public override float Height => height;
        public override Vector3 Offset => offset;
        public override bool Is2D => is2d;
    }
}