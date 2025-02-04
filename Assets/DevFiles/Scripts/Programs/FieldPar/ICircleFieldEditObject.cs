using UnityEngine;

namespace clrev01.Programs.FieldPar
{
    public interface ICircleFieldEditObject : IFieldEditObject
    {
        (float farRadius, float nearRadius, float angle, float rotate, float height, Vector3 offset) GetIndicateInfo();
    }
}