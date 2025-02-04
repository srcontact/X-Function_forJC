using static I2.Loc.ScriptLocalization;
using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.ClAction.ObjectSearch;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using System;
using static clrev01.Bases.UtlOfCL;
using static clrev01.Save.UtlOfVariable;

namespace clrev01.Programs.FuncPar
{
    [Serializable]
    [MemoryPackable()]
    public partial class GetTargetStatusValueFuncPar : FunctionFuncPar, IPGBFuncUnion, IGetStatusValueFuncPar
    {
        public override string BlockTypeStr => pgNodeName.getTargetStatus;
        public TgtStatusValueType statusType;
        public VariableDataNumericSet tgtVn = new() { };
        public VariableDataVector3Set tgtVv = new() { };
        public VariableDataLockOnGet sourceTgtV = new();
        public SearchTgtType aimingObjectType = SearchTgtType.Machine;
        public SpeedUnitType speedUnitType;

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (TgtStatusValueType* st = &statusType)
            fixed (SearchTgtType* aot = &aimingObjectType)
            fixed (SpeedUnitType* sut = &speedUnitType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_getTargetStatusValueFuncPar.statusType, pgNodeParDescription_getTargetStatusValueFuncPar.statusType);
                pgbepManager.SetPgbepEnum(typeof(TgtStatusValueType), (int*)st);

                pgbepManager.SetHeaderText(pgNodeParameter_getTargetStatusValueFuncPar.target, pgNodeParDescription_getTargetStatusValueFuncPar.target);
                sourceTgtV.IndicateWithIndex(pgbepManager);

                if (statusType is TgtStatusValueType.NumberOfAimingTgt)
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_getTargetStatusValueFuncPar.aimingObjectType, pgNodeParDescription_getTargetStatusValueFuncPar.aimingObjectType);
                    pgbepManager.SetPgbepFlagsEnum(typeof(SearchTgtType), (long*)aot);
                }

                switch (statusType)
                {
                    case TgtStatusValueType.Speed:
                    case TgtStatusValueType.HorizontalSpeed:
                    case TgtStatusValueType.VerticalSpeed:
                    case TgtStatusValueType.RelativeSpeed:
                    case TgtStatusValueType.RelativeHorizontalSpeed:
                    case TgtStatusValueType.RelativeVerticalSpeed:
                        pgbepManager.SetHeaderText(pgNodeParameter_getTargetStatusValueFuncPar.speedUnit, pgNodeParDescription_getTargetStatusValueFuncPar.speedUnit);
                        pgbepManager.SetPgbepEnum(typeof(SpeedUnitType), (int*)sut);
                        break;
                }

                var useVector3dTgt = IsUseVectorVariable();
                tgtVn.useVariable = !useVector3dTgt;
                tgtVv.useVariable = useVector3dTgt;

                pgbepManager.SetHeaderText(pgNodeParameter_getTargetStatusValueFuncPar.targetVariable, pgNodeParDescription_getTargetStatusValueFuncPar.targetVariable);
                if (useVector3dTgt) tgtVv.IndicateSwitchable(pgbepManager);
                else tgtVn.IndicateSwitchable(pgbepManager);
            }
        }
        private bool IsUseVectorVariable()
        {
            bool useVector3dTgt = false;
            switch (statusType)
            {
                case TgtStatusValueType.Position:
                case TgtStatusValueType.Position2D:
                case TgtStatusValueType.RelativePosition:
                case TgtStatusValueType.RelativePosition2D:
                case TgtStatusValueType.Rotation:
                case TgtStatusValueType.Rotation2D:
                case TgtStatusValueType.MoveVelocity:
                case TgtStatusValueType.MoveVelocity2D:
                case TgtStatusValueType.RelativeMoveVelocity:
                case TgtStatusValueType.RelativeMoveVelocity2D:
                case TgtStatusValueType.LandingPointNormal:
                    useVector3dTgt = true;
                    break;
            }
            return useVector3dTgt;
        }

        public void GetStatusValue(MachineLD ld)
        {
            if (tgtVn.useVariable)
            {
                var res = GetTargetNumericStatusValue(ld, statusType, sourceTgtV.GetUseValue(ld), speedUnitType, aimingObjectType);
                tgtVn.SetNumericValue(ld, res);
            }
            else if (tgtVv.useVariable)
            {
                var res = GetTargetVectorStatusValue(ld, statusType, sourceTgtV.GetUseValue(ld));
                tgtVv.SetVector3dValue(ld, res);
            }
        }

        public override string[] GetNodeFaceText()
        {
            var str1 = statusType switch
            {
                TgtStatusValueType.Speed or TgtStatusValueType.HorizontalSpeed
                    or TgtStatusValueType.VerticalSpeed or TgtStatusValueType.RelativeSpeed
                    or TgtStatusValueType.RelativeHorizontalSpeed or TgtStatusValueType.RelativeVerticalSpeed
                    => $"\nSU:{speedUnitType}",
                TgtStatusValueType.NumberOfAimingTgt => $"\nAOT:{GetEnumFlagText(typeof(SearchTgtType), (long)aimingObjectType, 2)}",
                _ => ""
            };
            var str2 = $"\nTgtV:{(IsUseVectorVariable() ? tgtVv.GetIndicateStr() : tgtVn.GetIndicateStr())}";
            return new[] { $"TGT:{sourceTgtV.GetIndicateStr()}\nST:{statusType}{str1}{str2}" };
        }
    }
}