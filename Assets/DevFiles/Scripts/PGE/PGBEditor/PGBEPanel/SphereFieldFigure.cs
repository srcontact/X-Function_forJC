using clrev01.PGE.FieldFigure;
using clrev01.Programs;
using clrev01.Programs.FieldPar;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public class SphereFieldFigure : FieldFigure<ISphereFieldEditObject>
    {
        [SerializeField]
        private CircleFieldPanel horizontalCirclePanel, verticalCirclePanel;
        [SerializeField]
        private ParameterInd farRadius, nearRadius, horizontalAngle, verticalAngle1, verticalAngle2, rotateX, rotateY, offsetX, offsetY, offsetZ;

        public override void SetIndicate(ISphereFieldEditObject searchFieldPar)
        {
            base.SetIndicate(searchFieldPar);
            if (searchFieldPar?.FieldType is not SearchFieldType.Sphere)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            var fieldPar = searchFieldPar.GetIndicateInfo();
            horizontalCirclePanel.SetCirclePos(fieldPar.farRadius, fieldPar.nearRadius, fieldPar.horizontalAngle, fieldPar.rotate.y,
                new Vector2(fieldPar.offset.x, fieldPar.offset.z), searchFieldPar, (false, true, false));
            verticalCirclePanel.SetSphereVPos(fieldPar.farRadius, fieldPar.nearRadius, fieldPar.verticalAngle1, fieldPar.verticalAngle2,
                fieldPar.rotate.x, fieldPar.offset.y, searchFieldPar, (true, false, true));
            farRadius.parameterStr = fieldPar.farRadius.ToString();
            nearRadius.parameterStr = fieldPar.nearRadius.ToString();
            horizontalAngle.parameterStr = fieldPar.horizontalAngle.ToString();
            verticalAngle1.parameterStr = fieldPar.verticalAngle1.ToString();
            verticalAngle2.parameterStr = fieldPar.verticalAngle2.ToString();
            rotateX.parameterStr = fieldPar.rotate.x.ToString();
            rotateY.parameterStr = fieldPar.rotate.y.ToString();
            offsetX.parameterStr = fieldPar.offset.x.ToString();
            offsetY.parameterStr = fieldPar.offset.y.ToString();
            offsetZ.parameterStr = fieldPar.offset.z.ToString();
        }
    }
}