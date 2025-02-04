using clrev01.Bases;
using clrev01.Programs.FieldPar;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace clrev01.PGE.PGBEditor.PGBEPanel
{
    public abstract class FieldFigure<FT> : BaseOfCL where FT : IFieldEditObject
    {
        [SerializeField]
        protected float screenSize = 10000;
        [SerializeField]
        protected List<GameObject> onOff2d3dObjs = new List<GameObject>();
        [SerializeField]
        private TextMeshProUGUI title;

        public virtual void SetIndicate(FT fieldPar)
        {
            if (fieldPar is not null) title.text = fieldPar.FieldFigureTitle;
        }
        protected void OnOff2d3d(bool is2d)
        {
            foreach (var go in onOff2d3dObjs)
            {
                go.SetActive(!is2d);
            }
        }
    }
}