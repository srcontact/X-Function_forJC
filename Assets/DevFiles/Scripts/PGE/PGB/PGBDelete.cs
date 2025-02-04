using UnityEngine;

namespace clrev01.PGE.PGB
{
    public partial class PGBlock2
    {
        public virtual void DeleteGo()
        {
            Debug.Log("delete__" + gameObject);
            //PGEM2.nowEditPD.PGList.Remove(pgbd);
            //PGEM2.PGBSetting();
        }
    }
}