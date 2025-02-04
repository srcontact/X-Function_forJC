using static I2.Loc.ScriptLocalization;
using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.ClAction.ObjectSearch;
using clrev01.PGE.PGBEditor;
using clrev01.PGE.VariableEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using System.Collections.Generic;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class AssessLockOnNumber : BranchFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.assessLockonNumber;
        public AssessLockOnType assessLockOnType;
        public VariableDataLockOnList targetList = new();
        public VariableDataNumericGet assessNumV = new() { constValue = 1 };
        public SearchTgtType tgtType = (SearchTgtType)int.MaxValue;
        public ComparatorType comparatorType = ComparatorType.Over;

        public enum AssessLockOnType
        {
            AssessAllList,
            AssessSelectList,
        }

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (AssessLockOnType* alt = &assessLockOnType)
            fixed (SearchTgtType* st = &tgtType)
            fixed (ComparatorType* ct = &comparatorType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_assessLockOnNumber.lockOnList, pgNodeParDescription_assessLockOnNumber.lockOnList);
                pgbepManager.SetPgbepEnum(typeof(AssessLockOnType), (int*)alt);
                if (assessLockOnType is AssessLockOnType.AssessSelectList) pgbepManager.SetPgbepVariable(targetList, false);
                pgbepManager.SetHeaderText(pgNodeParameter_assessLockOnNumber.targetType, pgNodeParDescription_assessLockOnNumber.targetType);
                pgbepManager.SetPgbepFlagsEnum(typeof(SearchTgtType), (long*)st);
                pgbepManager.SetHeaderText(pgNodeParameter_assessLockOnNumber.assessNumber, pgNodeParDescription_assessLockOnNumber.assessNumber);
                assessNumV.IndicateSwitchableInt(pgbepManager);
                pgbepManager.SetHeaderText(pgNodeParameter_assessLockOnNumber.comparatorType, pgNodeParDescription_assessLockOnNumber.comparatorType);
                pgbepManager.SetPgbepEnum(typeof(ComparatorType), (int*)ct);
            }
        }
        private List<ObjectSearchTgt> _countedList;
        public override bool BranchExecute(MachineLD ld)
        {
            var num = 0;
            switch (assessLockOnType)
            {
                case AssessLockOnType.AssessAllList:
                    if (_countedList == null) _countedList = new List<ObjectSearchTgt>();
                    else _countedList.Clear();
                    if (ld.variableValueDict.TryGetValue(VariableType.LockOnList, out var lld))
                    {
                        foreach (var variableValueBase in lld.Values)
                        {
                            var ll = variableValueBase as VariableValueLockOnList;
                            if (ll?.Value == null) continue;
                            foreach (var x in ll.Value)
                            {
                                if (x == null || (x.ObjectSearchType & tgtType) == 0 || _countedList.Contains(x)) continue;
                                _countedList.Add(x);
                                num++;
                            }
                        }
                    }
                    break;
                case AssessLockOnType.AssessSelectList:
                    var lockOnList = targetList.GetUseValue(ld);
                    if (lockOnList != null)
                    {
                        foreach (var x in lockOnList)
                        {
                            if (x != null && (x.ObjectSearchType & tgtType) != 0) num++;
                        }
                    }
                    break;
            }
            var useAssessNum = assessNumV.GetUseValueInt(ld);
            return comparatorType switch
            {
                ComparatorType.EqualTo => num == useAssessNum,
                ComparatorType.Over => num >= useAssessNum,
                ComparatorType.GreaterThan => num > useAssessNum,
                ComparatorType.Under => num < useAssessNum,
                ComparatorType.LessThan => num <= useAssessNum,
                _ => false
            };
        }
        private int GetLockOnNum(MachineLD ld)
        {
            var lockOnList = targetList.GetUseValue(ld);
            var num = 0;
            foreach (var x in lockOnList)
            {
                if ((x.ObjectSearchType & tgtType) != 0) num++;
            }
            return num;
        }

        public override string[] GetNodeFaceText()
        {
            var ll = assessLockOnType is AssessLockOnType.AssessSelectList ? $" LL:{targetList.GetIndicateStr()}" : "";
            return new[] { "Num", $"LT:{assessLockOnType}{ll} TT:{GetEnumFlagText(typeof(SearchTgtType), (long)tgtType, 2)} AN:{GetComparatorStr(comparatorType)}{assessNumV.GetIndicateStr()}" };
        }
    }
}