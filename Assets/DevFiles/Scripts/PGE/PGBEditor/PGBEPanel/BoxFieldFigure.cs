using clrev01.PGE.FieldFigure;
using clrev01.Programs;
using clrev01.Programs.FieldPar;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public class BoxFieldFigure : FieldFigure<IBoxFieldEditObject>
    {
        [SerializeField]
        private BoxFieldPanel horizontalBoxPanel, verticalBoxPanel;
        [SerializeField]
        private ParameterInd sizeX, sizeY, sizeZ, offsetX, offsetY, offsetZ, rotateX, rotateY, rotateZ;

        public override void SetIndicate(IBoxFieldEditObject searchFieldPar)
        {
            base.SetIndicate(searchFieldPar);
            if (searchFieldPar?.FieldType is not SearchFieldType.Box)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            OnOff2d3d(searchFieldPar.Is2D);
            var fieldPar = searchFieldPar.GetIndicateInfo();
            horizontalBoxPanel.SetBoxPos(
                new Vector2(fieldPar.size.x, fieldPar.size.z),
                new Vector2(fieldPar.offset.x, fieldPar.offset.z),
                searchFieldPar.Is2D ? 0 : fieldPar.rotate.x, -fieldPar.rotate.y,
                searchFieldPar,
                (false, true, false)
            );
            verticalBoxPanel.SetBoxPos(
                new Vector2(fieldPar.size.x, fieldPar.size.z),
                new Vector2(fieldPar.offset.x, fieldPar.offset.z),
                searchFieldPar.Is2D ? 0 : fieldPar.rotate.x, -fieldPar.rotate.y,
                searchFieldPar, (true, false, true)
            );
            sizeX.parameterStr = fieldPar.size.x.ToString();
            sizeY.parameterStr = fieldPar.size.y.ToString();
            sizeZ.parameterStr = fieldPar.size.z.ToString();
            offsetX.parameterStr = fieldPar.offset.x.ToString();
            offsetY.parameterStr = fieldPar.offset.y.ToString();
            offsetZ.parameterStr = fieldPar.offset.z.ToString();
            rotateX.parameterStr = fieldPar.rotate.x.ToString();
            rotateY.parameterStr = fieldPar.rotate.y.ToString();
            rotateZ.parameterStr = fieldPar.rotate.z.ToString();
        }
    }
}