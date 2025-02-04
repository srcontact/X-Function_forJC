using static I2.Loc.ScriptLocalization;
using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.ClAction.ObjectSearch;
using clrev01.PGE.PGBEditor;
using clrev01.PGE.VariableEditor;
using clrev01.Programs.FieldPar;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save.VariableData;
using MemoryPack;
using System;
using System.Collections.Generic;
using static clrev01.Bases.UtlOfCL;
using static clrev01.Programs.UtlOfProgram;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class LockOnFuncPar : FunctionFuncPar, IPGBFuncUnion
    {
        [MemoryPackIgnore]
        public const int numberOfMultiLockOnMax = 12;
        public override string BlockTypeStr => pgNodeName.lockon;
        public VariableDataLockOnSet lockOnList = new();
        public IdentificationType identificationType = IdentificationType.Enemy;
        public ObjType searchObjType = ObjType.Machine;
        public LockOnDistancePriorityType lockOnDistancePriorityType;
        public ISearchFieldUnion searchFieldPar = new BoxSearchFieldParVariable();
        public VariableDataNumericGet numberOfMultiLockOnV = new() { constValue = 1 };
        public LockOnAngleOfMovementToSelfType lockOnAngleOfMovementToSelfType;
        public VariableDataNumericGet angleOfMovementToSelfV = new() { constValue = 90 };
        public bool useIgnoreLockOn;
        public VariableDataLockOnList ignoreLockOnList = new();

        [MemoryPackIgnore]
        public ObjectSearchTgt[] lockOnResult { get; set; }

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (IdentificationType* it = &identificationType)
            fixed (ObjType* ot = &searchObjType)
            fixed (LockOnDistancePriorityType* ldt = &lockOnDistancePriorityType)
            fixed (LockOnAngleOfMovementToSelfType* lat = &lockOnAngleOfMovementToSelfType)
            fixed (bool* uil = &useIgnoreLockOn)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_lockOnFuncPar.lockOnList, pgNodeParDescription_lockOnFuncPar.lockOnList);
                pgbepManager.SetPgbepVariable(lockOnList, false, new List<VariableType> { VariableType.LockOn, VariableType.LockOnList });
                pgbepManager.SetHeaderText(pgNodeParameter_lockOnFuncPar.identificationType, pgNodeParDescription_lockOnFuncPar.identificationType);
                pgbepManager.SetPgbepFlagsEnum(typeof(IdentificationType), (long*)it);
                pgbepManager.SetHeaderText(pgNodeParameter_lockOnFuncPar.objectType, pgNodeParDescription_lockOnFuncPar.objectType);
                pgbepManager.SetPgbepFlagsEnum(typeof(ObjType), (long*)ot);
                pgbepManager.SetHeaderText(pgNodeParameter_lockOnFuncPar.distancePriority, pgNodeParDescription_lockOnFuncPar.distancePriority);
                pgbepManager.SetPgbepEnum(typeof(LockOnDistancePriorityType), (int*)ldt);
                pgbepManager.SetHeaderText(pgNodeParameter_lockOnFuncPar.angleOfMovementToSelf, pgNodeParDescription_lockOnFuncPar.angleOfMovementToSelf);
                pgbepManager.SetPgbepEnum(typeof(LockOnAngleOfMovementToSelfType), (int*)lat);
                if (lockOnAngleOfMovementToSelfType is not LockOnAngleOfMovementToSelfType.None) angleOfMovementToSelfV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(0, 180));
                pgbepManager.SetHeaderText(pgNodeParameter_lockOnFuncPar.searchField, pgNodeParDescription_lockOnFuncPar.searchField);
                pgbepManager.SetPgbepField(searchFieldPar, (res) => { searchFieldPar = (ISearchFieldUnion)res; });
                if (lockOnList.variableType is VariableType.LockOnList)
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_lockOnFuncPar.numberOfMultiLockOn, pgNodeParDescription_lockOnFuncPar.numberOfMultiLockOn);
                    numberOfMultiLockOnV.IndicateSwitchableInt(pgbepManager, new PgbepManager.IntSliderSettingPar(1, numberOfMultiLockOnMax));
                }
                pgbepManager.SetHeaderText(pgNodeParameter_lockOnFuncPar.ignoreLockOn, pgNodeParDescription_lockOnFuncPar.ignoreLockOn);
                pgbepManager.SetPgbepToggle(uil);
                if (useIgnoreLockOn) pgbepManager.SetPgbepVariable(ignoreLockOnList, false);
            }
        }

        public void LockOn(MachineLD ld)
        {
            lockOnResult ??= new ObjectSearchTgt[numberOfMultiLockOnV.GetUseValueInt(ld, 1, LockOnFuncPar.numberOfMultiLockOnMax)];
            var ignoreList = useIgnoreLockOn ? ignoreLockOnList.GetUseValue(ld) : null;
            searchFieldPar.LockOn(
                ld,
                identificationType,
                searchObjType,
                lockOnDistancePriorityType,
                ld.hd.transform,
                lockOnAngleOfMovementToSelfType,
                angleOfMovementToSelfV.GetUseValueFloat(ld, 0, 180),
                ld.hd.teamID,
                lockOnResult,
                ignoreList
            );
            lockOnList.SetLockOnList(ld, lockOnResult);
        }

        public override string[] GetNodeFaceText()
        {
            var it = GetEnumFlagText(typeof(IdentificationType), (long)identificationType, 1);
            var ot = GetEnumFlagText(typeof(ObjType), (long)searchObjType, 2);
            var ldp = GetEnumFlagText(typeof(LockOnDistancePriorityType), (int)lockOnDistancePriorityType);
            var ams = lockOnAngleOfMovementToSelfType switch
            {
                LockOnAngleOfMovementToSelfType.None => "None",
                LockOnAngleOfMovementToSelfType.SmallerThan => $"<{angleOfMovementToSelfV.GetIndicateStr("°")}",
                LockOnAngleOfMovementToSelfType.BiggerThan => $">{angleOfMovementToSelfV.GetIndicateStr("°")}",
                _ => throw new ArgumentOutOfRangeException()
            };
            var mln = numberOfMultiLockOnV.GetIndicateStr();

            return new[] { $"LL:{lockOnList.GetIndicateStr()} IT:{it} OT:{ot} DP:{ldp} AoMtS:{ams} NoML:{mln} {searchFieldPar.GetFieldShortText()}" };
        }
        public override IFieldEditObject GetNodeFaceIFieldEditObject()
        {
            return searchFieldPar;
        }
        public override float?[] GetNodeFaceValue()
        {
            return new float?[] { 0 };
        }
    }
}