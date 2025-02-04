using clrev01.Bases;
using clrev01.Programs;
using clrev01.Programs.FieldPar;
using System.Collections.Generic;
using UnityEngine;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.PGE.PGBEditor.PGBEDetailMenu
{
    public abstract class FieldDetailBase<FT, FI> : BaseOfCL
        where FT : IFieldEditObject
        where FI : Field3dIndicater<FT>
    {
        [SerializeField]
        protected FI indicater3D;
        [SerializeField]
        protected ScaleIndicater scaleIndicater;
        [SerializeField]
        protected float default3dFieldScale = 12.5f;
        [SerializeField]
        protected FT fieldPar;
        [SerializeField]
        protected List<ScaleLines> scaleLinesList = new List<ScaleLines>();
        protected abstract bool is2D { get; }
        protected abstract SearchFieldType FieldType { get; }


        public void Activate(IFieldEditObject par)
        {
            indicater3D.gameObject.SetActive(par.FieldType == FieldType);
            if (par is not FT ft)
            {
                fieldPar = default;
                return;
            }
            fieldPar = ft;
            UpdateIndicator();
        }
        public void UpdateIndicator()
        {
            if (fieldPar == null) return;
            var magnificationNum = fieldPar.CalcMagnificationNum(scaleIndicater.defaultBounds, default3dFieldScale);
            scaleIndicater.SetIndicatePar(magnificationNum);
            foreach (var scaleLine in scaleLinesList)
            {
                scaleLine.IndicateScales(magnificationNum, default3dFieldScale);
            }
            indicater3D.SetIndicatePar(fieldPar);
        }
    }
}