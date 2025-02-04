using clrev01.Bases;
using clrev01.PGE.PGBEditor.PGBEDetailMenu;
using clrev01.Programs;
using clrev01.Programs.FieldPar;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE.FieldFigure
{
    public class FieldPanel : BaseOfCL
    {
        [SerializeField]
        private Transform scaler, axisObj;
        [SerializeField]
        private RectTransform background;
        [SerializeField]
        private BoxField3dIndicater boxField3dIndicator;
        [SerializeField]
        private CircleField3dIndicater circleField3dIndicator;
        [SerializeField]
        private SphereField3dIndicater sphereField3dIndicator;
        [SerializeField]
        private float fieldObjScale = 1;

        public void SetIndicate(IFieldEditObject fieldEditObject)
        {
            var rect = background.rect;
            var bounds = new Bounds(rect.min, Vector3.zero);
            bounds.Expand(rect.max);
            var panelScale = Mathf.Min(rect.width, rect.height) / 2;
            scaler.localScale = Vector3.one / fieldEditObject.GetIndicateBoundsMax() * panelScale * fieldObjScale;
            axisObj.localScale = Vector3.one * panelScale;
            boxField3dIndicator.gameObject.SetActive(false);
            circleField3dIndicator.gameObject.SetActive(false);
            sphereField3dIndicator.gameObject.SetActive(false);
            switch (fieldEditObject.FieldType)
            {
                case SearchFieldType.Box:
                    boxField3dIndicator.gameObject.SetActive(true);
                    boxField3dIndicator.SetIndicatePar(fieldEditObject as IBoxFieldEditObject);
                    break;
                case SearchFieldType.Circle:
                    circleField3dIndicator.gameObject.SetActive(true);
                    circleField3dIndicator.SetIndicatePar(fieldEditObject as ICircleFieldEditObject);
                    break;
                case SearchFieldType.Sphere:
                    sphereField3dIndicator.gameObject.SetActive(true);
                    sphereField3dIndicator.SetIndicatePar(fieldEditObject as ISphereFieldEditObject);
                    break;
            }
        }
    }
}