using clrev01.ClAction.ObjectSearch;
using UnityEngine;

public class SearchTestTgt01 : MonoBehaviour
{
    [SerializeField]
    ObjectSearchTgt objectSearchTgt;

    private void Update()
    {
        ObjectSearch.Inst.RegisterTgt(objectSearchTgt);
    }
}