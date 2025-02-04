using clrev01.Programs.FieldPar;
using System.Collections.Generic;
using UnityEngine;

namespace clrev01.PGE.PGBEditor.PGBEDetailMenu
{
    public class CircleField3dIndicater : Field3dIndicater<ICircleFieldEditObject>
    {
        [SerializeField]
        private int spritVNum = 10, spritHNum = 10, spritSNum = 10;
        [SerializeField]
        float nowMaxRadius, nowMinRadius, nowAngle, nowRotate, nowHeight;
        private List<Vector2> _plxyMin = new();
        private List<Vector2> _plxyMax = new();
        private List<Vector3> _vpl = new();
        private List<Vector3> _hpl1 = new();
        private List<Vector3> _hpl2 = new();
        private List<List<Vector2>> _spll2 = new();
        private List<Vector3> _spl1 = new();
        private List<Vector3> _spl2 = new();
        protected override int lineNum => 5;

        private void OnEnable()
        {
            nowMaxRadius = nowMinRadius = nowAngle = nowRotate = nowHeight = 0;
            lpos = Vector3.zero;
        }

        protected override void UpdateField()
        {
            if (tgtFieldPar == null) return;
            var fieldPar = tgtFieldPar.GetIndicateInfo();
            nowMaxRadius = Mathf.Lerp(nowMaxRadius, fieldPar.farRadius, slerpRate);
            nowMinRadius = Mathf.Lerp(nowMinRadius, fieldPar.nearRadius, slerpRate);
            nowAngle = Mathf.Lerp(nowAngle, fieldPar.angle, slerpRate);
            nowHeight = Mathf.Lerp(nowHeight, !tgtFieldPar.Is2D ? fieldPar.height : 0, slerpRate);

            Vector3 v = fieldPar.offset;
            if (tgtFieldPar.Is2D) v.y = 0;
            lpos = Vector3.Slerp(lpos, v, slerpRate);

            nowRotate = Mathf.Lerp(nowRotate, fieldPar.rotate, slerpRate);
            lrot = Quaternion.Euler(0, nowRotate, 0);

            Rendering(nowMaxRadius, nowMinRadius, nowAngle, nowHeight);
        }
        private void Rendering(float farRadius, float nearRadius, float angle, float height)
        {
            _plxyMin.Clear();
            _plxyMax.Clear();

            for (int i = 0; i <= spritVNum; i++)
            {
                float a = Mathf.Lerp(-angle / 2, angle / 2, (float)i / spritVNum) * Mathf.Deg2Rad;
                if (nearRadius > 0) _plxyMin.Add(new Vector2(nearRadius * Mathf.Sin(a), nearRadius * Mathf.Cos(a)));
                _plxyMax.Add(new Vector2(farRadius * Mathf.Sin(a), farRadius * Mathf.Cos(a)));
            }

            VerticalLineDraw(nearRadius, height, _plxyMin, _plxyMax);
            HorizontalLineDraw(nearRadius, angle, height, _plxyMin, _plxyMax);
            SideLinesDraw(nearRadius, angle, height, _plxyMin, _plxyMax);
        }

        private void VerticalLineDraw(
            float nearRadius, float height, List<Vector2> plxyMin, List<Vector2> plxyMax)
        {
            _vpl.Clear();
            int xor = 1;
            for (int i = 0; i < plxyMax.Count; i++)
            {
                Vector3 minP, maxP;
                if (nearRadius > 0) minP = plxyMin[i];
                else minP = Vector2.zero;
                maxP = plxyMax[i];
                _vpl.Add(new Vector3(minP.x, height / 2 * xor, minP.y));
                _vpl.Add(new Vector3(maxP.x, height / 2 * xor, maxP.y));
                _vpl.Add(new Vector3(maxP.x, -height / 2 * xor, maxP.y));
                _vpl.Add(new Vector3(minP.x, -height / 2 * xor, minP.y));
                if (nearRadius > 0)
                {
                    _vpl.Add(new Vector3(minP.x, height / 2 * xor, minP.y));
                }
                else xor *= -1;
            }
            SetPoints(_vpl, 0);
        }
        private void HorizontalLineDraw(
            float nearRadius, float angle, float height, List<Vector2> plxyMin, List<Vector2> plxyMax)
        {
            List<Vector3> hplTemp;
            _hpl1.Clear();
            _hpl2.Clear();
            for (int i = 0; i <= spritHNum; i++)
            {
                float h = Mathf.Lerp(-height / 2, height / 2, (float)i / spritHNum);
                hplTemp = _hpl1;
                for (int j = 0; j < plxyMax.Count; j++)
                {
                    hplTemp.Add(new Vector3(
                        plxyMax[j].x, h, plxyMax[j].y
                    ));
                }
                if (angle >= 360) hplTemp = _hpl2;
                for (int j = plxyMax.Count - 1; j >= 0; j--)
                {
                    if (nearRadius > 0)
                    {
                        hplTemp.Add(new Vector3(
                            plxyMin[j].x, h, plxyMin[j].y
                        ));
                    }
                    else hplTemp.Add(new Vector3(0, h, 0));
                }
                if (angle < 360)
                {
                    hplTemp.Add(new Vector3(
                        plxyMax[0].x, h, plxyMax[0].y
                    ));
                }
            }
            SetPoints(_hpl1, 1);
            SetPoints(_hpl2, 2);
        }
        private void SideLinesDraw(
            float nearRadius, float angle, float height, List<Vector2> plxyMin, List<Vector2> plxyMax)
        {
            float sh = height / 2;
            _spll2.Clear();
            for (int i = 1; i < spritSNum; i++)
            {
                float lerp = Mathf.Lerp(0, 1, (float)i / spritSNum);
                List<Vector2> spl2 = new List<Vector2>();
                for (int j = 0; j < plxyMax.Count; j++)
                {
                    Vector3 minv = nearRadius > 0 ? plxyMin[j] : Vector2.zero;
                    spl2.Add(Vector2.Lerp(minv, plxyMax[j], lerp));
                }
                _spll2.Add(spl2);
            }
            if (angle < 360)
            {
                List<Vector3> spl = new List<Vector3>();
                foreach (var pl in _spll2)
                {
                    for (int j = 0; j < pl.Count; j++)
                    {
                        spl.Add(new Vector3(pl[j].x, sh, pl[j].y));
                    }
                    for (int j = pl.Count - 1; j >= 0; j--)
                    {
                        spl.Add(new Vector3(pl[j].x, -sh, pl[j].y));
                    }
                    spl.Add(new Vector3(pl[0].x, sh, pl[0].y));
                }
                SetPoints(spl, 3);
                SetPoints(null, 4);
            }
            else
            {
                _spl1.Clear();
                _spl2.Clear();
                bool xor = false;
                foreach (var pl in _spll2)
                {
                    for (int j = 0; j < pl.Count; j++)
                    {
                        int jj = xor ? j : pl.Count - j - 1;
                        _spl1.Add(new Vector3(pl[jj].x, sh, pl[jj].y));
                        _spl2.Add(new Vector3(pl[jj].x, -sh, pl[jj].y));
                    }
                    xor = !xor;
                }
                SetPoints(_spl1, 3);
                SetPoints(_spl2, 4);
            }
        }
    }
}