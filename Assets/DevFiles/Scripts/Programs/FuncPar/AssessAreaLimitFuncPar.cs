using static I2.Loc.ScriptLocalization;
using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FieldPar;
using clrev01.Programs.FuncPar.FuncParType;
using MemoryPack;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class AssessAreaLimitFuncPar : BranchFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.assessAreaLimit;

        public ISearchFieldUnion searchFieldPar = new BoxSearchFieldParVariable() { is2D = true };


        public override void SetPointers(PgbepManager pgbepManager)
        {
            pgbepManager.SetHeaderText(pgNodeParameter_assessAreaLimitFuncPar.searchField, pgNodeParDescription_assessAreaLimitFuncPar.searchField);
            pgbepManager.SetPgbepField(searchFieldPar, (res) => { searchFieldPar = (ISearchFieldUnion)res; }, true, true);
        }

        public override bool BranchExecute(MachineLD ld)
        {
            searchFieldPar.GetValueFromVariable(ld);
            searchFieldPar.CalcField(ld.hd.transform);
            searchFieldPar.CalcAABB(out var bounds);
            Vector3 max = bounds.max;
            Vector3 min = bounds.min;
            max.y = min.y = 0;
            return !ACM.areaLimitBounds.Contains(max) || !ACM.areaLimitBounds.Contains(min);
        }
        public override string[] GetNodeFaceText()
        {
            return new[] { searchFieldPar.GetFieldShortText() };
        }
        public override IFieldEditObject GetNodeFaceIFieldEditObject()
        {
            return searchFieldPar;
        }
    }
}