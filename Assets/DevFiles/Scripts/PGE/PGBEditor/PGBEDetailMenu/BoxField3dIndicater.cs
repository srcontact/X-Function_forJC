using clrev01.Programs.FieldPar;
using System.Collections.Generic;
using UnityEngine;

namespace clrev01.PGE.PGBEditor.PGBEDetailMenu
{
    public class BoxField3dIndicater : Field3dIndicater<IBoxFieldEditObject>
    {
        [SerializeField]
        protected int spritNum = 10;
        [SerializeField]
        Vector3 nowSize, nowOffset;
        private List<Vector3> _plAll = new();
        protected override int lineNum => 3;

        private void OnEnable()
        {
            nowSize = Vector3.zero;
            nowOffset = Vector3.zero;
        }
        protected override void UpdateField()
        {
            if (tgtFieldPar == null) return;
            var fieldPar = tgtFieldPar.GetIndicateInfo();
            Vector3 offset = fieldPar.offset, rotate = fieldPar.rotate, size = fieldPar.size;
            bool is2d = tgtFieldPar.Is2D;
            if (is2d) offset.y = rotate.x = size.y = 0;
            nowOffset = Vector3.Slerp(nowOffset, offset, slerpRate);
            lrot = Quaternion.Slerp(lrot, Quaternion.Euler(rotate), slerpRate);
            nowSize = Vector3.Slerp(nowSize, size, slerpRate);
            Rendering(nowSize, nowOffset);
        }
        private void Rendering(Vector3 fieldSize, Vector3 fieldOffset)
        {
            Vector3 xp = new Vector3(fieldSize.x / 2, 0, 0);
            Vector3 yp = new Vector3(0, fieldSize.y / 2, 0);
            Vector3 zp = new Vector3(0, 0, fieldSize.z / 2);
            RenderSquare(zp, xp, yp, fieldOffset, 0);
            RenderSquare(xp, yp, zp, fieldOffset, 1);
            RenderSquare(yp, zp, xp, fieldOffset, 2);
        }
        void RenderSquare(
            Vector3 sizeAxis0,
            Vector3 sizeAxis1,
            Vector3 sizeAxis2,
            Vector3 fieldOffset, int tgtNum)
        {
            _plAll.Clear();
            for (int i1 = 0; i1 <= spritNum; i1++)
            {
                Vector3 offset = Vector3.Lerp(-sizeAxis0, sizeAxis0, (float)i1 / spritNum) + fieldOffset;

                _plAll.Add(offset - sizeAxis1 - sizeAxis2);
                _plAll.Add(offset + sizeAxis1 - sizeAxis2);
                _plAll.Add(offset + sizeAxis1 + sizeAxis2);
                _plAll.Add(offset - sizeAxis1 + sizeAxis2);
                _plAll.Add(offset - sizeAxis1 - sizeAxis2);
            }
            SetPoints(_plAll, tgtNum);
        }
    }
}