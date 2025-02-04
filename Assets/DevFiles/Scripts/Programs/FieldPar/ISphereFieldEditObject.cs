using UnityEngine;

namespace clrev01.Programs.FieldPar
{
    public interface ISphereFieldEditObject : IFieldEditObject
    {
        public (float farRadius, float nearRadius, float horizontalAngle, float verticalAngle1, float verticalAngle2, Vector2 rotate, Vector3 offset) GetIndicateInfo();
    }
}