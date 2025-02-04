using clrev01.Bases;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CircleLinesTest : BaseOfCL
{
    [SerializeField]
    LineRenderer lineRenderer1, lineRenderer2, lineRenderer3;
    [SerializeField]
    float radiusMin = 5, radiusMax = 10;
    [SerializeField, Range(0f, 360f)]
    float angle = 360;
    [SerializeField, Range(-180f, 180f)]
    float rotate;
    [SerializeField]
    float height = 10;
    [SerializeField]
    int spritVNum = 10, spritHNum = 10;

    private void OnValidate()
    {
        Rendering();
    }

    private void Rendering()
    {
        List<Vector2> plxyMin = new List<Vector2>();
        List<Vector2> plxyMax = new List<Vector2>();

        for (int i = 0; i <= spritVNum; i++)
        {
            float a = Mathf.Lerp(-angle / 2, angle / 2, (float)i / spritVNum) * Mathf.Deg2Rad;
            if (radiusMin > 0) plxyMin.Add(new Vector2(radiusMin * Mathf.Cos(a), radiusMin * Mathf.Sin(a)));
            plxyMax.Add(new Vector2(radiusMax * Mathf.Cos(a), radiusMax * Mathf.Sin(a)));
        }

        List<Vector3> vpl = new List<Vector3>();
        int xor = 1;
        for (int i = 0; i < plxyMax.Count; i++)
        {
            Vector3 minP, maxP;
            if (radiusMin > 0) minP = plxyMin[i];
            else minP = Vector2.zero;
            maxP = plxyMax[i];
            vpl.Add(new Vector3(minP.x, height / 2 * xor, minP.y));
            vpl.Add(new Vector3(maxP.x, height / 2 * xor, maxP.y));
            vpl.Add(new Vector3(maxP.x, -height / 2 * xor, maxP.y));
            vpl.Add(new Vector3(minP.x, -height / 2 * xor, minP.y));
            if (radiusMin > 0)
            {
                vpl.Add(new Vector3(minP.x, height / 2 * xor, minP.y));
            }
            else xor *= -1;
        }
        lineRenderer1.positionCount = vpl.Count;
        lineRenderer1.SetPositions(vpl.ToArray());

        List<Vector3> hpl = null;
        List<Vector3> hpl1 = new List<Vector3>();
        List<Vector3> hpl2 = new List<Vector3>();
        for (int i = 0; i <= spritHNum; i++)
        {
            float h = Mathf.Lerp(-height / 2, height / 2, (float)i / spritHNum);
            hpl = hpl1;
            for (int j = 0; j < plxyMax.Count; j++)
            {
                hpl.Add(new Vector3(
                    plxyMax[j].x, h, plxyMax[j].y
                ));
            }
            if (angle >= 360) hpl = hpl2;
            for (int j = plxyMax.Count - 1; j >= 0; j--)
            {
                if (radiusMin > 0)
                {
                    hpl.Add(new Vector3(
                        plxyMin[j].x, h, plxyMin[j].y
                    ));
                }
                else hpl.Add(new Vector3(0, h, 0));
            }
            if (angle < 360)
            {
                hpl.Add(new Vector3(
                    plxyMax[0].x, h, plxyMax[0].y
                ));
            }
        }
        lineRenderer2.positionCount = hpl1.Count;
        lineRenderer2.SetPositions(hpl1.ToArray());
        lineRenderer3.positionCount = hpl2.Count;
        lineRenderer3.SetPositions(hpl2.ToArray());
    }

    private void GetArc(float zAngle, float radius, float hPos, bool spinNormal, List<Vector3> pl)
    {
        float xyr;
        int spin = 1;
        if (!spinNormal) spin *= -1;
        for (int xyi = 0; xyi < spritVNum + 1; xyi++)
        {
            xyr = Mathf.Lerp(-angle * spin / 2, angle * spin / 2, (float)xyi / spritVNum) * Mathf.Deg2Rad;
            pl.Add(new Vector3(
                radius * Mathf.Sin(zAngle) * Mathf.Cos(xyr),
                hPos,
                radius * Mathf.Sin(zAngle) * Mathf.Sin(xyr)));
        }
    }
}