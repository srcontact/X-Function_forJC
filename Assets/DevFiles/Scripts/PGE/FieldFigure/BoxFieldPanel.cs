using clrev01.Bases;
using clrev01.Programs.FieldPar;
using UnityEngine;

namespace clrev01.PGE.FieldFigure
{
    public class BoxFieldPanel : BaseOfCL
    {
        [SerializeField]
        private RectTransform boxRect;
        [SerializeField]
        private float minMaxSizeScreenSizeRatio = 0.05f;
        [SerializeField]
        private float fieldObjScale = 0.8f;

        public void SetBoxPos(Vector2 size, Vector2 offset, float rotX, float rotZ, IFieldEditObject fieldPar, (bool x, bool y, bool z)? ignoreAxis = null)
        {
            size /= 2;
            var center = Vector2.one / 2;
            var boundsMax = fieldPar.GetIndicateBoundsMax() / fieldObjScale;
            if (size.x > 0 && size.x < boundsMax * minMaxSizeScreenSizeRatio) size.x = boundsMax * minMaxSizeScreenSizeRatio;
            if (size.y > 0 && size.y < boundsMax * minMaxSizeScreenSizeRatio) size.y = boundsMax * minMaxSizeScreenSizeRatio;
            var max = (offset + size) / boundsMax + center;
            var min = (offset - size) / boundsMax + center;
            boxRect.anchorMax = max;
            boxRect.anchorMin = min;
            boxRect.parent.localRotation = Quaternion.Euler(0, 0, rotZ);
            boxRect.localRotation = Quaternion.Euler(rotX, 0, 0);
        }
    }
}