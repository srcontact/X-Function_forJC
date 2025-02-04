using clrev01.Bases;
using clrev01.Programs.FieldPar;
using UnityEngine;
using UnityEngine.UI;

namespace clrev01.PGE.FieldFigure
{
    public class CircleFieldPanel : BaseOfCL
    {
        [SerializeField]
        private RectTransform fieldPanelRect;
        [SerializeField]
        private Image circleMaxImage;
        [SerializeField]
        private RectTransform circleMinRect;
        [SerializeField]
        private float minMaxRadiusScreenSizeRatio = 0.05f, minMinRadiusScreenSizeRatio = 0.025f;
        [SerializeField]
        private float fieldObjScale = 0.8f;

        public void SetCirclePos(float maxR, float minR, float angle, float rotate, Vector2 offset, IFieldEditObject fieldPar, (bool x, bool y, bool z)? ignoreAxis = null)
        {
            circleMaxImage.fillAmount = angle / 360f;
            fieldPanelRect.localRotation = Quaternion.Euler(0, 0, -rotate + angle / 2);
            var center = Vector2.one / 2;
            var boundsMax = fieldPar.GetIndicateBoundsMax() / fieldObjScale;
            if (maxR > 0 && maxR < boundsMax * minMaxRadiusScreenSizeRatio) maxR = boundsMax * minMaxRadiusScreenSizeRatio;
            if (minR > 0 && minR < boundsMax * minMinRadiusScreenSizeRatio) minR = boundsMax * minMinRadiusScreenSizeRatio;
            fieldPanelRect.anchorMax = (offset + maxR * Vector2.one) / boundsMax / 2 + center;
            fieldPanelRect.anchorMin = (offset - maxR * Vector2.one) / boundsMax / 2 + center;
            circleMinRect.localScale = Vector3.one * minR / maxR;
        }

        public void SetSphereVPos(float maxR, float minR, float angle1, float angle2, float vRotate, float offsetY, IFieldEditObject fieldPar, (bool x, bool y, bool z)? ignoreAxis = null)
        {
            var boundsMax = fieldPar.GetIndicateBoundsMax() / fieldObjScale;
            if (maxR > 0 && maxR < boundsMax * minMaxRadiusScreenSizeRatio) maxR = boundsMax * minMaxRadiusScreenSizeRatio;
            if (minR > 0 && minR < boundsMax * minMinRadiusScreenSizeRatio) minR = boundsMax * minMinRadiusScreenSizeRatio;
            var center = offsetY / boundsMax + 0.5f;
            var size = maxR / boundsMax;
            fieldPanelRect.anchorMax = new Vector2(1, size + center);
            fieldPanelRect.anchorMin = new Vector2(0, -size + center);
            circleMinRect.localScale = Vector3.one * minR / maxR;
            float rotate = Mathf.Max(angle1, angle2) - 90 - vRotate;
            float angle = Mathf.Abs(angle1 - angle2) / 360;
            circleMaxImage.transform.localRotation = Quaternion.Euler(0, 0, rotate);
            circleMaxImage.fillAmount = angle;
        }
    }
}