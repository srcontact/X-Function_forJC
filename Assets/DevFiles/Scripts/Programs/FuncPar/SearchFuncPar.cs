using static I2.Loc.ScriptLocalization;
using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FieldPar;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using System;
using static clrev01.Bases.UtlOfCL;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class SearchFuncPar : BranchFuncPar, IPGBFuncUnion
    {
        [MemoryPackIgnore]
        public const int numberOfMultiLockOnMax = 12;
        public override string BlockTypeStr => pgNodeName.search;

        public IdentificationType identificationType = IdentificationType.Enemy;
        public ObjType searchObjType = ObjType.Machine;
        public ISearchFieldUnion searchFieldPar = new BoxSearchFieldParVariable();
        public LockOnAngleOfMovementToSelfType angleOfMovementToSelfType;
        public VariableDataNumericGet angleOfMovementToSelfV = new() { constValue = 90 };
        public ComparatorType numberOfSearchTgtComparatorType = ComparatorType.Over;
        public VariableDataNumericGet numberOfSearchTgtV = new() { constValue = 1 };
        public bool useIgnoreLockOn;
        public VariableDataLockOnList ignoreLockOnList = new();

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (IdentificationType* it = &identificationType)
            fixed (ObjType* ot = &searchObjType)
            fixed (LockOnAngleOfMovementToSelfType* lat = &angleOfMovementToSelfType)
            fixed (ComparatorType* ct = &numberOfSearchTgtComparatorType)
            fixed (bool* uil = &useIgnoreLockOn)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_searchFuncPar.identificationType, pgNodeParDescription_searchFuncPar.identificationType);
                pgbepManager.SetPgbepFlagsEnum(typeof(IdentificationType), (long*)it);
                pgbepManager.SetHeaderText(pgNodeParameter_searchFuncPar.searchObjectType, pgNodeParDescription_searchFuncPar.searchObjectType);
                pgbepManager.SetPgbepFlagsEnum(typeof(ObjType), (long*)ot);
                pgbepManager.SetHeaderText(pgNodeParameter_searchFuncPar.angleOfMovementToSelf, pgNodeParDescription_searchFuncPar.angleOfMovementToSelf);
                pgbepManager.SetPgbepEnum(typeof(LockOnAngleOfMovementToSelfType), (int*)lat);
                if (angleOfMovementToSelfType is not LockOnAngleOfMovementToSelfType.None) angleOfMovementToSelfV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(0, 180));
                pgbepManager.SetHeaderText(pgNodeParameter_searchFuncPar.searchField, pgNodeParDescription_searchFuncPar.searchField);
                pgbepManager.SetPgbepField(searchFieldPar, (res) => { searchFieldPar = (ISearchFieldUnion)res; });
                pgbepManager.SetHeaderText(pgNodeParameter_searchFuncPar.numberOfSearchTgt, pgNodeParDescription_searchFuncPar.numberOfSearchTgt);
                numberOfSearchTgtV.IndicateSwitchableInt(pgbepManager, new PgbepManager.IntSliderSettingPar(1, numberOfMultiLockOnMax));
                pgbepManager.SetPgbepEnum(typeof(ComparatorType), (int*)ct);
                pgbepManager.SetHeaderText(pgNodeParameter_searchFuncPar.ignoreLockOn, pgNodeParDescription_searchFuncPar.ignoreLockOn);
                pgbepManager.SetPgbepToggle(uil);
                if (useIgnoreLockOn) pgbepManager.SetPgbepVariable(ignoreLockOnList, false);
            }
        }

        public override bool BranchExecute(MachineLD ld)
        {
            var ignoreList = useIgnoreLockOn ? ignoreLockOnList.GetUseValue(ld) : null;
            return searchFieldPar.Search(
                ld,
                identificationType,
                searchObjType,
                ld.hd.transform,
                angleOfMovementToSelfType,
                angleOfMovementToSelfV.GetUseValueFloat(ld, (float?)0, 180),
                numberOfSearchTgtComparatorType,
                numberOfSearchTgtV.GetUseValueInt(ld, 1, numberOfMultiLockOnMax),
                ld.hd.teamID,
                ignoreList
            );
        }
        public override string[] GetNodeFaceText()
        {
            var it = GetEnumFlagText(typeof(IdentificationType), (long)identificationType, 1);
            var ot = GetEnumFlagText(typeof(ObjType), (long)searchObjType, 2);
            var ams = angleOfMovementToSelfType switch
            {
                LockOnAngleOfMovementToSelfType.None => "None",
                LockOnAngleOfMovementToSelfType.SmallerThan => $"<{angleOfMovementToSelfV.GetIndicateStr("°")}",
                LockOnAngleOfMovementToSelfType.BiggerThan => $">{angleOfMovementToSelfV.GetIndicateStr("°")}",
                _ => throw new ArgumentOutOfRangeException()
            };
            var nst = numberOfSearchTgtV.GetIndicateStr();
            var nstct = numberOfSearchTgtComparatorType switch
            {
                ComparatorType.EqualTo => "=",
                ComparatorType.Over => ">=",
                ComparatorType.GreaterThan => ">",
                ComparatorType.Under => "<=",
                ComparatorType.LessThan => "<",
                _ => throw new ArgumentOutOfRangeException()
            };

            return new[] { $"IT:{it} OT:{ot} AoMtS:{ams} NoST:{nstct}{nst} {searchFieldPar.GetFieldShortText()}" };
        }
        public override IFieldEditObject GetNodeFaceIFieldEditObject()
        {
            return searchFieldPar;
        }
    }
}