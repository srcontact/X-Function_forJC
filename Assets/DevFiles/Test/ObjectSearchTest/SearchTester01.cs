using clrev01.ClAction.Machines;
using clrev01.Programs.FieldPar;
using UnityEngine;
using Random = UnityEngine.Random;

public class SearchTester01 : MonoBehaviour
{
    [SerializeField]
    bool boxtest, circletest, spheretest;
    [SerializeField]
    BoxSearchFieldParVariable boxSearchFieldPar;
    [SerializeField]
    CircleSearchFieldParVariable circleSearchFieldPar;
    [SerializeField]
    SphereSearchFieldParVariable sphereSearchFieldPar;
    [SerializeField]
    SearchMark marker;
    [SerializeField]
    GameObject markerParent;
    [SerializeField]
    int markNum = 100;
    [SerializeField]
    float randSize = 100;

    void Update()
    {
        if (boxtest)
        {
            Mark(boxSearchFieldPar);
        }
        else if (circletest)
        {
            Mark(circleSearchFieldPar);
        }
        else if (spheretest)
        {
            Mark(sphereSearchFieldPar);
        }
    }

    Bounds bounds;
    private MachineLD dammy = new();
    void Mark(ISearchFieldUnion searchField)
    {
        searchField.GetValueFromVariable(dammy);
        searchField.CalcField(transform);
        searchField.CalcAABB(out bounds);
        Debug.Log(bounds);
        for (int i = 0; i < markNum; i++)
        {
            Vector3 p = Random.insideUnitSphere * randSize;
            SearchMark mark;
            if (bounds.Contains(p) && searchField.CheckInField(p))
            {
                mark = Instantiate(marker, p, Quaternion.identity, markerParent.transform);
                mark.isDestroy = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}