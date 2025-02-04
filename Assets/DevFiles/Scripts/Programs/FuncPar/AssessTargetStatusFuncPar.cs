using static I2.Loc.ScriptLocalization;
using clrev01.Bases;
using clrev01.ClAction.Bullets;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using System;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class AssessTargetStatusFuncPar : BranchFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.assessTargetStatus;
        public VariableDataLockOnGet targetList = new();
        public StatusType statusType;
        public ComparisonType comparisonType;
        public VariableDataNumericGet assessmentValueV = new();
        public ComparatorType assessmentType;
        public SpeedUnitType speedUnitType;

        public enum StatusType
        {
            HealthPoint = 0,
            Heat = 1,
            Energy = 2,
            Speed = 3,
            Shield = 4,
            Impact = 5,
        }

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (StatusType* st = &statusType)
            fixed (ComparisonType* ct = &comparisonType)
            fixed (ComparatorType* at = &assessmentType)
            fixed (SpeedUnitType* sut = &speedUnitType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_assessTargetStatusFuncPar.target, pgNodeParDescription_assessTargetStatusFuncPar.target);
                targetList.IndicateWithIndex(pgbepManager);
                pgbepManager.SetHeaderText(pgNodeParameter_assessTargetStatusFuncPar.status, pgNodeParDescription_assessTargetStatusFuncPar.status);
                pgbepManager.SetPgbepEnum(typeof(StatusType), (int*)st);
                if (statusType is not StatusType.Speed)
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_assessTargetStatusFuncPar.comparisonType, pgNodeParDescription_assessTargetStatusFuncPar.comparisonType);
                    pgbepManager.SetPgbepEnum(typeof(ComparisonType), (int*)ct);
                }
                else
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_assessTargetStatusFuncPar.speedUnit, pgNodeParDescription_assessTargetStatusFuncPar.speedUnit);
                    pgbepManager.SetPgbepEnum(typeof(SpeedUnitType), (int*)sut);
                }
                pgbepManager.SetHeaderText(pgNodeParameter_assessTargetStatusFuncPar.value, pgNodeParDescription_assessTargetStatusFuncPar.value);
                assessmentValueV.IndicateSwitchableFloat(pgbepManager, comparisonType is ComparisonType.Percentage ? new PgbepManager.FloatSliderSettingPar(0, 100) : null);
                pgbepManager.SetHeaderText(pgNodeParameter_assessTargetStatusFuncPar.@operator, pgNodeParDescription_assessTargetStatusFuncPar.@operator);
                pgbepManager.SetPgbepEnum(typeof(ComparatorType), (int*)at);
            }
        }

        public override bool BranchExecute(MachineLD ld)
        {
            var tgt = targetList.GetUseValue(ld);
            if (tgt == null) return false;
            float nowPar = 0;
            switch (tgt.hardBase, statusType)
            {
                case (_, StatusType.Speed):
                    nowPar = tgt.hardBase.rigidBody.linearVelocity.magnitude * speedUnitType.GetSpeedUnitRatio();
                    break;
                case (MachineHD hd, _):
                    var tld = hd.ld;
                    switch (statusType)
                    {
                        case StatusType.HealthPoint:
                            nowPar = tld.cd.maxHearthPoint - tld.statePar.damage;
                            break;
                        case StatusType.Heat:
                            nowPar = tld.statePar.heat;
                            break;
                        case StatusType.Energy:
                            nowPar = tld.powerPlantData.energyCapacity - tld.statePar.energyUsed;
                            break;
                        case StatusType.Shield:
                            nowPar = tld.shieldCd.healthPoint - tld.statePar.shieldDamage;
                            break;
                        case StatusType.Impact:
                            nowPar = tld.statePar.impact;
                            break;
                        default:
                            return false;
                    }
                    break;
                case (BulletHD hd, _):
                    var bld = hd.ld;
                    switch (statusType)
                    {
                        case StatusType.HealthPoint:
                            nowPar = bld.cd.BulletHealthPoint - bld.damage;
                            break;
                    }
                    break;
            }

            float av = assessmentValueV.GetUseValueFloat(ld);
            switch (comparisonType, statusType)
            {
                case (ComparisonType.RealNumber, _):
                case (_, StatusType.Speed):
                    break;
                case (ComparisonType.Percentage, _):
                    switch (statusType)
                    {
                        case StatusType.HealthPoint:
                            av = av / 100 * ld.cd.maxHearthPoint;
                            break;
                        case StatusType.Heat:
                            av = av / 100 * ld.cd.allowableTemperature;
                            break;
                        case StatusType.Energy:
                            av = av / 100 * ld.powerPlantData.energyCapacity;
                            break;
                        case StatusType.Shield:
                            av = av / 100 * ld.shieldCd.healthPoint;
                            break;
                        case StatusType.Impact:
                            av = av / 100 * ld.cd.baseStability;
                            break;
                    }
                    break;
            }
            bool res;
            switch (assessmentType)
            {
                case ComparatorType.EqualTo:
                    res = Math.Abs(nowPar - av) < float.Epsilon;
                    break;
                case ComparatorType.Over:
                    res = nowPar >= av;
                    break;
                case ComparatorType.GreaterThan:
                    res = nowPar > av;
                    break;
                case ComparatorType.Under:
                    res = nowPar <= av;
                    break;
                case ComparatorType.LessThan:
                    res = nowPar < av;
                    break;
                default:
                    return false;
            }

            return res;
        }
        public override string[] GetNodeFaceText()
        {
            var comparisonStr = comparisonType switch
            {
                ComparisonType.RealNumber => "",
                ComparisonType.Percentage => "%",
                _ => throw new ArgumentOutOfRangeException()
            };
            var comparatorStr = GetComparatorStr(assessmentType);
            return new[] { $"{targetList.GetIndicateStr()}\n{statusType} {comparatorStr} {assessmentValueV.GetIndicateStr()}{comparisonStr}" };
        }
    }
}