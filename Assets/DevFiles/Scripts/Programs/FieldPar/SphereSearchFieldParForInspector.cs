using System;
using UnityEngine;

namespace clrev01.Programs.FieldPar
{
    [Serializable]
    public class SphereSearchFieldParForInspector : SphereSearchFieldPar
    {
        [SerializeField]
        public float farRadius = 100;
        [SerializeField]
        public float nearRadius;
        [SerializeField]
        public float horizontalAngle = 360;
        [SerializeField]
        public float verticalAngle1 = 90;
        [SerializeField]
        public float verticalAngle2 = -90;
        [SerializeField]
        public Vector3 rotate;
        [SerializeField]
        public Vector3 offset;
        [SerializeField]
        public bool is2d;

        public override float FarRadius => farRadius;
        public override float NearRadius => nearRadius;
        public override float HorizontalAngle => horizontalAngle;
        public override float VerticalAngle1 => verticalAngle1;
        public override float VerticalAngle2 => verticalAngle2;
        public override Vector3 Rotate => rotate;
        public override Vector3 Offset => offset;
        public override bool Is2D => is2d;
    }
}