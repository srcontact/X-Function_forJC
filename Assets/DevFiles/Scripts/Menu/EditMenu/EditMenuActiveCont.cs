using clrev01.Bases;
using System.Collections.Generic;
using UnityEngine;

namespace clrev01.Menu.EditMenu
{
    public class EditMenuActiveCont : BaseOfCL
    {
        [SerializeField]
        List<GameObject> tgtObjs = new List<GameObject>();

        private void OnEnable()
        {
            ActivateExe();
        }
        public void ActivateExe()
        {
            for (int i = 0; i < tgtObjs.Count; i++)
            {
                tgtObjs[i].SetActive(StaticInfo.Inst.nowEditMech != null);
            }
        }
    }
}