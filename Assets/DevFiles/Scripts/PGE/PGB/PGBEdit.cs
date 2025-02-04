using clrev01.Programs;
using System;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE.PGB
{
    public partial class PGBlock2
    {
        public virtual void EditGo(Action afterEdit = null)
        {
            Debug.Log("edit__" + gameObject);
            PGEM2.editMenu.OpenEditor(index, EditPg, afterEdit);
        }
    }
}