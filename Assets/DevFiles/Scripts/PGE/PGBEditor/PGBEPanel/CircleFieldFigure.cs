using clrev01.PGE.FieldFigure;
using clrev01.Programs;
using clrev01.Programs.FieldPar;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public class CircleFieldFigure : FieldFigure<ICircleFieldEditObject>
    {
        [SerializeField]
        private CircleFieldPanel horizontalCirclePanel;
        [SerializeField]
        private BoxFieldPanel verticalBoxPanel;
        [SerializeField]
        private ParameterInd farRadius, nearRadius, angle, rotate, height, offsetX, offsetY, offsetZ;

        public override void SetIndicate(ICircleFieldEditObject searchFieldPar)
        {
            base.SetIndicate(searchFieldPar);
            if (searchFieldPar?.FieldType is not SearchFieldType.Circle)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            var fieldPar = searchFieldPar.GetIndicateInfo();
            OnOff2d3d(searchFieldPar.Is2D);
            horizontalCirclePanel.SetCirclePos(
                fieldPar.farRadius,
                fieldPar.nearRadius,
                fieldPar.angle,
                fieldPar.rotate,
                new Vector2(fieldPar.offset.x, fieldPar.offset.z),
                searchFieldPar,
                (false, true, false)
            );
            verticalBoxPanel.SetBoxPos(
                new Vector2(fieldPar.farRadius, fieldPar.height),
                new Vector2(0, fieldPar.offset.y),
                0, 0,
                searchFieldPar,
                (true, false, true)
            );
            farRadius.parameterStr = fieldPar.farRadius.ToString();
            nearRadius.parameterStr = fieldPar.nearRadius.ToString();
            angle.parameterStr = fieldPar.angle.ToString();
            rotate.parameterStr = fieldPar.rotate.ToString();
            height.parameterStr = fieldPar.height.ToString();
            offsetX.parameterStr = fieldPar.offset.x.ToString();
            offsetY.parameterStr = fieldPar.offset.y.ToString();
            offsetZ.parameterStr = fieldPar.offset.z.ToString();
        }
    }
}