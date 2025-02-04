using clrev01.Bases;
using System.Collections.Generic;
using UnityEngine;

public class BoxLinesTest : BaseOfCL
{
    [SerializeField]
    LineRenderer lineRenderer1, lineRenderer2, lineRenderer3;
    [SerializeField]
    int spritNum = 10;
    public Vector3 fieldSize = Vector3.one;

    private void OnValidate()
    {
        Rendering();
    }

    private void Rendering()
    {
        Vector3 xp = new Vector3(fieldSize.x / 2, 0, 0);
        Vector3 yp = new Vector3(0, fieldSize.y / 2, 0);
        Vector3 zp = new Vector3(0, 0, fieldSize.z / 2);
        RenderSquare(zp, xp, yp, lineRenderer1);
        RenderSquare(xp, yp, zp, lineRenderer2);
        RenderSquare(yp, zp, xp, lineRenderer3);
    }
    void RenderSquare(Vector3 sizeAxis0, Vector3 sizeAxis1, Vector3 sizeAxis2, LineRenderer lineRenderer)
    {
        List<Vector3> plAll = new List<Vector3>();
        for (int i1 = 0; i1 <= spritNum; i1++)
        {
            Vector3 offset = Vector3.Lerp(-sizeAxis0, sizeAxis0, (float)i1 / spritNum);

            plAll.Add(offset - sizeAxis1 - sizeAxis2);
            plAll.Add(offset + sizeAxis1 - sizeAxis2);
            plAll.Add(offset + sizeAxis1 + sizeAxis2);
            plAll.Add(offset - sizeAxis1 + sizeAxis2);
            plAll.Add(offset - sizeAxis1 - sizeAxis2);
        }
        lineRenderer.positionCount = plAll.Count;
        lineRenderer.SetPositions(plAll.ToArray());
    }
}