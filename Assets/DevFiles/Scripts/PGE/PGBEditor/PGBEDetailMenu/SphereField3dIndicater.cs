using System.Collections.Generic;
using clrev01.Programs.FieldPar;
using UnityEngine;

namespace clrev01.PGE.PGBEditor.PGBEDetailMenu
{
    public class SphereField3dIndicater : Field3dIndicater<ISphereFieldEditObject>
    {
        [SerializeField, Header("※偶数値では不要な線が出てしまうので、奇数を使用すること")]
        private int splitNum = 15;
        
        [SerializeField]
        private float nowAngleZ1, nowAngleZ2, nowAngleXy, nowRadiusMax, nowRadiusMin;

        private readonly List<List<Vector3>> _pllXyMax = new();
        private readonly List<List<Vector3>> _pllXyMin = new();
        private readonly List<List<Vector3>> _pllZMax = new();
        private readonly List<List<Vector3>> _pllZMin = new();
        private readonly List<Vector3> _spl = new();
        private readonly List<Vector3> _plAll = new();
        private readonly List<Vector3> _plAll2 = new();

        protected override int lineNum => 5;
        
        bool isMinOverZero => nowRadiusMin > 0;
        bool xyClose => nowAngleXy >= 360;
        bool z1Close => Mathf.Abs(nowAngleZ1) >= 90;
        bool z2Close => Mathf.Abs(nowAngleZ2) >= 90;

        private void OnEnable()
        {
            nowAngleZ1 = nowAngleZ2 = nowAngleXy = nowRadiusMax = nowRadiusMin = 0;
            lpos = Vector3.zero;
            lrot = Quaternion.identity;

            // Initialize lists with the appropriate capacity
            InitializeList(_pllXyMax, splitNum + 1);
            InitializeList(_pllXyMin, splitNum + 1);
            InitializeList(_pllZMax, splitNum + 1);
            InitializeList(_pllZMin, splitNum + 1);
        }

        private void InitializeList(List<List<Vector3>> list, int capacity)
        {
            list.Clear();
            for (int i = 0; i < capacity; i++)
            {
                list.Add(new List<Vector3>());
            }
        }

        protected override void UpdateField()
        {
            if (tgtFieldPar == null) return;
            var fieldPar = tgtFieldPar.GetIndicateInfo();
            lpos = Vector3.Slerp(lpos, fieldPar.offset, slerpRate);
            lrot = Quaternion.Slerp(lrot, Quaternion.Euler(fieldPar.rotate), slerpRate);
            nowAngleZ1 = Mathf.Lerp(nowAngleZ1, fieldPar.verticalAngle1, slerpRate);
            nowAngleZ2 = Mathf.Lerp(nowAngleZ2, fieldPar.verticalAngle2, slerpRate);
            nowAngleXy = Mathf.Lerp(nowAngleXy, fieldPar.horizontalAngle, slerpRate);
            nowRadiusMax = Mathf.Lerp(nowRadiusMax, fieldPar.farRadius, slerpRate);
            nowRadiusMin = Mathf.Lerp(nowRadiusMin, fieldPar.nearRadius, slerpRate);
            Rendering(nowAngleZ1, nowAngleZ2, nowAngleXy, nowRadiusMax, nowRadiusMin);
        }

        private void Rendering(float angleZ1, float angleZ2, float angleXy, float radiusMax, float radiusMin)
        {
            ClearAndResizeList(_pllXyMax, splitNum + 1);
            ClearAndResizeList(_pllXyMin, splitNum + 1);
            ClearAndResizeList(_pllZMax, splitNum + 1);
            ClearAndResizeList(_pllZMin, splitNum + 1);

            GetPoints(angleZ1, angleZ2, angleXy, radiusMax, radiusMin, _pllXyMax, _pllXyMin);
            GetVPoints(_pllXyMax, _pllZMax);
            if (isMinOverZero) GetVPoints(_pllXyMin, _pllZMin);

            if (xyClose)
            {
                if (!z1Close)
                {
                    _spl.Clear();
                    GetSideLineList(_pllXyMax, _pllXyMin, 0, _spl);
                    SetPoints(_spl, 3);
                }
                else SetPoints(null, 3);
                if (!z2Close)
                {
                    _spl.Clear();
                    GetSideLineList(_pllXyMax, _pllXyMin, _pllXyMax.Count - 1, _spl);
                    SetPoints(_spl, 4);
                }
                else SetPoints(null, 4);
            }
            else
            {
                SetPoints(null, 4);
                _spl.Clear();
                for (int i = 1; i < splitNum; i++)
                {
                    float lerp = Mathf.Lerp(0, 1, (float)i / splitNum);
                    GetSideLine(_pllXyMax, _pllXyMin, 0, lerp, _spl);
                    GetSideLine(_pllZMax, _pllZMin, _pllZMax.Count - 1, lerp, _spl);
                    GetSideLine(_pllXyMax, _pllXyMin, _pllXyMax.Count - 1, lerp, _spl, true);
                    GetSideLine(_pllZMax, _pllZMin, 0, lerp, _spl, true);
                }
                SetPoints(_spl, 3);
            }
            RenderSphere(_pllXyMax, _pllXyMin, _pllZMax, _pllZMin);
        }

        private void ClearAndResizeList(List<List<Vector3>> list, int capacity)
        {
            for (int i = 0; i < capacity; i++)
            {
                if (i < list.Count)
                {
                    list[i].Clear();
                }
                else
                {
                    list.Add(new List<Vector3>());
                }
            }
        }

        private void GetSideLineList(List<List<Vector3>> pllMax, List<List<Vector3>> pllMin, int n, List<Vector3> targetList)
        {
            for (int i = 1; i < splitNum; i++)
            {
                float lerp = Mathf.Lerp(0, 1, (float)i / splitNum);
                GetSideLine(pllMax, pllMin, n, lerp, targetList);
            }
        }

        private void GetSideLine(List<List<Vector3>> pllMax, List<List<Vector3>> pllMin, int n, float lerp, List<Vector3> targetList, bool reverse = false)
        {
            for (int i = 0; i < pllMax[n].Count; i++)
            {
                int ii = reverse ? pllMax[n].Count - i - 1 : i;
                targetList.Add(Vector3.Lerp(
                    isMinOverZero ? pllMin[n][ii] : Vector3.zero,
                    pllMax[n][ii],
                    lerp));
            }
        }

        private void RenderSphere(List<List<Vector3>> pllXyMax, List<List<Vector3>> pllXyMin, List<List<Vector3>> pllZMax, List<List<Vector3>> pllZMin)
        {
            _plAll.Clear();
            _plAll2.Clear();
            for (int i = 0; i < pllXyMax.Count; i++)
            {
                if (!xyClose)
                {
                    if (isMinOverZero)
                    {
                        pllXyMax[i][0] = pllXyMin[i][0];
                        pllXyMax[i].Add(pllXyMin[i][pllXyMin[i].Count - 1]);
                    }
                    else
                    {
                        pllXyMax[i][0] = Vector3.zero;
                        pllXyMax[i].Add(Vector3.zero);
                    }
                }
                if (i % 2 != 0) pllXyMax[i].Reverse();
                _plAll.AddRange(pllXyMax[i]);
            }
            for (int i = 0; i < pllXyMin.Count; i++)
            {
                if (i % 2 != 0) pllXyMin[i].Reverse();
                _plAll2.AddRange(pllXyMin[i]);
            }
            for (int i = 0; i < pllZMax.Count; i++)
            {
                if (!z1Close)
                {
                    if (isMinOverZero)
                    {
                        pllZMax[i][0] = pllZMin[i][0];
                    }
                    else
                    {
                        pllZMax[i][0] = Vector3.zero;
                    }
                }
                if (!z2Close)
                {
                    if (isMinOverZero)
                    {
                        pllZMax[i].Add(pllZMin[i][pllZMin[i].Count - 1]);
                    }
                    else
                    {
                        pllZMax[i].Add(Vector3.zero);
                    }
                }
                if (i % 2 == 0) pllZMax[i].Reverse();
                _plAll.AddRange(pllZMax[i]);
            }
            for (int i = 0; i < pllZMin.Count; i++)
            {
                if (i % 2 == 0) pllZMin[i].Reverse();
                _plAll2.AddRange(pllZMin[i]);
            }
            SetPoints(_plAll, 0);
            SetPoints(_plAll2, 1);
        }

        private void GetPoints(float angleZ1, float angleZ2, float angleXy, float radiusMax, float radiusMin, List<List<Vector3>> pllMax, List<List<Vector3>> pllMin)
        {
            for (int zi = 0; zi < splitNum + 1; zi++)
            {
                float zAngle = Mathf.Lerp(angleZ1 + 90, angleZ2 + 90, (float)zi / splitNum) * Mathf.Deg2Rad;
                if (isMinOverZero)
                {
                    List<Vector3> plMin = pllMin[zi];
                    GetArc(zAngle, radiusMin, angleXy, true, plMin);
                }
                List<Vector3> plMax = pllMax[zi];
                GetArc(zAngle, radiusMax, angleXy, true, plMax);
            }
        }

        void GetVPoints(List<List<Vector3>> hPll, List<List<Vector3>> vPll)
        {
            ClearAndResizeList(vPll, hPll[0].Count);
            for (int lli = 0; lli < hPll[0].Count; lli++)
            {
                vPll[lli].Clear();
                for (int li = 0; li < hPll.Count; li++)
                {
                    vPll[lli].Add(hPll[li][lli]);
                }
            }
        }

        private void GetArc(float zAngle, float radius, float angleXY, bool spinNormal, List<Vector3> pl)
        {
            float xyr;
            float zPos = -radius * Mathf.Cos(zAngle);
            int spin = 1;
            if (!spinNormal) spin *= -1;
            for (int xyi = 0; xyi < splitNum + 1; xyi++)
            {
                xyr = Mathf.Lerp(-angleXY * spin / 2, angleXY * spin / 2, (float)xyi / splitNum) * Mathf.Deg2Rad;
                pl.Add(new Vector3(
                    radius * Mathf.Sin(zAngle) * Mathf.Sin(xyr),
                    zPos,
                    radius * Mathf.Sin(zAngle) * Mathf.Cos(xyr)));
            }
        }
    }
}
