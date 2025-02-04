using UnityEngine;

namespace clrev01.Programs.FieldPar
{
    public interface IBoxFieldEditObject : IFieldEditObject
    {
        (Vector3 size, Vector3 offset, Vector3 rotate) GetIndicateInfo();
    }
}