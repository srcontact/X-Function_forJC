using static I2.Loc.ScriptLocalization;
using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.ClAction.ObjectSearch;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class AssessNumberOfAimingMeFuncPar : BranchFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.assessNumberOfAimingMe;
        public SearchTgtType aimingObjectType = SearchTgtType.Machine;
        public VariableDataNumericGet assessNumberV = new() { constValue = 1 };
        public ComparatorType comparatorType = ComparatorType.Over;

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (SearchTgtType* stt = &aimingObjectType)
            fixed (ComparatorType* ct = &comparatorType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_assessNumberOfAimingMeFuncPar.aimingObjectType, pgNodeParDescription_assessNumberOfAimingMeFuncPar.aimingObjectType);
                pgbepManager.SetPgbepFlagsEnum(typeof(SearchTgtType), (long*)stt);
                pgbepManager.SetHeaderText(pgNodeParameter_assessNumberOfAimingMeFuncPar.assessNumber, pgNodeParDescription_assessNumberOfAimingMeFuncPar.assessNumber);
                assessNumberV.IndicateSwitchableInt(pgbepManager);
                pgbepManager.SetHeaderText(pgNodeParameter_assessNumberOfAimingMeFuncPar.comparatorType, pgNodeParDescription_assessNumberOfAimingMeFuncPar.comparatorType);
                pgbepManager.SetPgbepEnum(typeof(ComparatorType), (int*)ct);
            }
        }
        public override bool BranchExecute(MachineLD ld)
        {
            if (ld.hd.objectSearchTgt == null && ld.hd.objectSearchTgt.aimingAtMeDict == null) return false;
            var num = 0;
            foreach (var x in ld.hd.objectSearchTgt.aimingAtMeDict)
            {
                if ((x.Value.ObjectSearchType & aimingObjectType) != 0) num++;
            }
            var assessNumber = assessNumberV.GetUseValueFloat(ld);
            switch (comparatorType)
            {
                case ComparatorType.EqualTo:
                    return Mathf.Approximately(num, assessNumber);
                case ComparatorType.Over:
                    return assessNumber <= num;
                case ComparatorType.GreaterThan:
                    return assessNumber < num;
                case ComparatorType.Under:
                    return assessNumber >= num;
                case ComparatorType.LessThan:
                    return assessNumber > num;
                default:
                    return false;
            }
        }
        public override string[] GetNodeFaceText()
        {
            var aot = GetEnumFlagText(typeof(SearchTgtType), (long)aimingObjectType, 2);
            return new[] { "Num", $"AOT:{aot} AN:{GetComparatorStr(comparatorType)}{assessNumberV.GetIndicateStr()}" };
        }
    }
}