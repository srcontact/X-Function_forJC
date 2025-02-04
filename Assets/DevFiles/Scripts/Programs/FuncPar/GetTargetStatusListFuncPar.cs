using static I2.Loc.ScriptLocalization;
using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.ClAction.ObjectSearch;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using static clrev01.Bases.UtlOfCL;
using static clrev01.Save.UtlOfVariable;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable]
    public partial class GetTargetStatusListFuncPar : FunctionFuncPar, IPGBFuncUnion, IGetStatusValueFuncPar
    {
        public override string BlockTypeStr => pgNodeName.getTargetStatusList;
        public TgtStatusValueType statusType;
        public VariableDataNumericList tgtVn = new() { };
        public VariableDataVector3DList tgtVv = new() { };
        public VariableDataLockOnList sourceTgtV = new();
        public SearchTgtType aimingObjectType = SearchTgtType.Machine;
        public SpeedUnitType speedUnitType;
        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (TgtStatusValueType* st = &statusType)
            fixed (SearchTgtType* aot = &aimingObjectType)
            fixed (SpeedUnitType* sut = &speedUnitType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_getTargetStatusListFuncPar.statusType, pgNodeParDescription_getTargetStatusListFuncPar.statusType);
                pgbepManager.SetPgbepEnum(typeof(TgtStatusValueType), (int*)st);

                pgbepManager.SetHeaderText(pgNodeParameter_getTargetStatusListFuncPar.target, pgNodeParDescription_getTargetStatusListFuncPar.target);
                pgbepManager.SetPgbepVariable(sourceTgtV, false);

                if (statusType is TgtStatusValueType.NumberOfAimingTgt)
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_getTargetStatusListFuncPar.aimingObjectType, pgNodeParDescription_getTargetStatusListFuncPar.aimingObjectType);
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
                        pgbepManager.SetHeaderText(pgNodeParameter_getTargetStatusListFuncPar.speedUnit, pgNodeParDescription_getTargetStatusListFuncPar.speedUnit);
                        pgbepManager.SetPgbepEnum(typeof(SpeedUnitType), (int*)sut);
                        break;
                }

                var useVector3dTgt = IsUseVectorVariable();
                tgtVn.useVariable = !useVector3dTgt;
                tgtVv.useVariable = useVector3dTgt;

                pgbepManager.SetHeaderText(pgNodeParameter_getTargetStatusListFuncPar.targetVariable, pgNodeParDescription_getTargetStatusListFuncPar.targetVariable);
                if (useVector3dTgt) pgbepManager.SetPgbepVariable(tgtVv, false);
                else pgbepManager.SetPgbepVariable(tgtVn, false);
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
                tgtVn.SetValue(ld, sourceTgtV.GetUseValue(ld).ConvertAll(x => GetTargetNumericStatusValue(ld, statusType, x, speedUnitType, aimingObjectType)));
            }
            else if (tgtVv.useVariable)
            {
                tgtVv.SetValue(ld, sourceTgtV.GetUseValue(ld).ConvertAll(x => GetTargetVectorStatusValue(ld, statusType, x)));
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
            var str2 = $"\nTgtV:[{(IsUseVectorVariable() ? tgtVv.name : tgtVn.name)}]";
            return new[] { $"TGT:{sourceTgtV.GetIndicateStr()}\nST:{statusType}{str1}{str2}" };
        }
    }
}