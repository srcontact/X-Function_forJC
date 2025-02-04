using clrev01.Bases;
using System.Collections.Generic;
using UnityEngine;

public class SphereLinesTest : BaseOfCL
{
    [SerializeField]
    LineRenderer lineRenderer, lineRenderer2;
    [SerializeField]
    float radiusMax, radiusMin, angleXY, angleZ1, angleZ2;
    [SerializeField]
    int spritNum = 100;

    bool isMinOverZero => radiusMin > 0;

    private void Update()
    {
        Rendering();
    }
    private void OnValidate()
    {
        Rendering();
    }
    private void Rendering()
    {
        List<List<Vector3>> pllXyMax = new List<List<Vector3>>();
        List<List<Vector3>> pllXyMin = new List<List<Vector3>>();
        List<List<Vector3>> pllZMax = new List<List<Vector3>>();
        List<List<Vector3>> pllZMin = new List<List<Vector3>>();
        GetPoints(pllXyMax, pllXyMin);
        GetVPoints(pllXyMax, pllZMax);
        if (isMinOverZero) GetVPoints(pllXyMin, pllZMin);
        RenderSphere(pllXyMax, pllXyMin, pllZMax, pllZMin);
    }

    private void RenderSphere(List<List<Vector3>> pllXyMax, List<List<Vector3>> pllXyMin, List<List<Vector3>> pllZMax, List<List<Vector3>> pllZMin)
    {
        bool xyClose = angleXY >= 360;
        bool z1Close = Mathf.Abs(angleZ1) >= 90;
        bool z2Close = Mathf.Abs(angleZ2) >= 90;
        List<Vector3> plAll = new List<Vector3>();
        List<Vector3> plAll2 = new List<Vector3>();
        for (int i = 0; i < pllXyMax.Count; i++)
        {
            if (!xyClose)
            {
                if (isMinOverZero)
                {
                    pllXyMax[i].Insert(0, pllXyMin[i][0]);
                    pllXyMax[i].Add(pllXyMin[i][pllXyMin.Count - 1]);
                }
                else
                {
                    pllXyMax[i].Insert(0, Vector3.zero);
                    pllXyMax[i].Add(Vector3.zero);
                }
            }
            if (i % 2 != 0) pllXyMax[i].Reverse();
            plAll.AddRange(pllXyMax[i].ToArray());
        }
        for (int i = 0; i < pllXyMin.Count; i++)
        {
            if (i % 2 != 0) pllXyMin[i].Reverse();
            plAll2.AddRange(pllXyMin[i].ToArray());
        }
        for (int i = 0; i < pllZMax.Count; i++)
        {
            if (!z1Close)
            {
                if (isMinOverZero)
                {
                    pllZMax[i].Insert(0, pllZMin[i][0]);
                }
                else
                {
                    pllZMax[i].Insert(0, Vector3.zero);
                }
            }
            if (!z2Close)
            {
                if (isMinOverZero)
                {
                    pllZMax[i].Add(pllZMin[i][pllZMin.Count - 1]);
                }
                else
                {
                    pllZMax[i].Add(Vector3.zero);
                }
            }
            if (i % 2 == 0) pllZMax[i].Reverse();
            plAll.AddRange(pllZMax[i].ToArray());
        }
        for (int i = 0; i < pllZMin.Count; i++)
        {
            if (i % 2 == 0) pllZMin[i].Reverse();
            plAll2.AddRange(pllZMin[i].ToArray());
        }
        lineRenderer.positionCount = plAll.Count;
        lineRenderer.SetPositions(plAll.ToArray());
        lineRenderer2.positionCount = plAll2.Count;
        lineRenderer2.SetPositions(plAll2.ToArray());
    }
    private void GetPoints(List<List<Vector3>> pllMax, List<List<Vector3>> pllMin)
    {
        for (int zi = 0; zi < spritNum + 1; zi++)
        {
            float zAngle = Mathf.Lerp(angleZ1 + 90, angleZ2 + 90, (float)zi / spritNum) * Mathf.Deg2Rad;
            if (isMinOverZero)
            {
                List<Vector3> plMin = new List<Vector3>();
                GetArc(zAngle, radiusMin, true, plMin);
                pllMin.Add(plMin);
            }
            List<Vector3> plMax = new List<Vector3>();
            GetArc(zAngle, radiusMax, true, plMax);
            pllMax.Add(plMax);
        }
    }
    void GetVPoints(List<List<Vector3>> hPll, List<List<Vector3>> vPll)
    {
        int llc = hPll[0].Count;
        int lc = hPll.Count;
        for (int lli = 0; lli < llc; lli++)
        {
            List<Vector3> pl = new List<Vector3>();
            for (int li = 0; li < lc; li++)
            {
                pl.Add(hPll[li][lli]);
            }
            vPll.Add(pl);
        }
    }
    private void GetArc(float zAngle, float radius, bool spinNormal, List<Vector3> pl)
    {
        float xyr;
        float zPos = -radius * Mathf.Cos(zAngle);
        int spin = 1;
        if (!spinNormal) spin *= -1;
        for (int xyi = 0; xyi < spritNum + 1; xyi++)
        {
            xyr = Mathf.Lerp(-angleXY * spin / 2, angleXY * spin / 2, (float)xyi / spritNum) * Mathf.Deg2Rad;
            pl.Add(new Vector3(
                radius * Mathf.Sin(zAngle) * Mathf.Cos(xyr),
                zPos,
                radius * Mathf.Sin(zAngle) * Mathf.Sin(xyr)));
        }
    }
}