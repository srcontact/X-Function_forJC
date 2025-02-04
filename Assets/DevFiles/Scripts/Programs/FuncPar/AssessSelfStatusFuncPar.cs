using static I2.Loc.ScriptLocalization;
using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using System;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Programs.FuncPar
{
    [Serializable]
    [MemoryPackable()]
    public partial class AssessSelfStatusFuncPar : BranchFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.assessSelfStatus;

        public StatusType statusType;
        public ComparisonType comparisonType;
        public VariableDataNumericGet assessmentValueV = new();
        public ComparatorType assessmentType;
        public SpeedUnitType speedUnitType;
        public int weapon;

        public enum StatusType
        {
            HealthPoint = 0,
            Heat = 1,
            Energy = 2,
            Speed = 3,
            Shield = 4,
            Impact = 5,
            WeaponAmo = 100,
            OptionalPartsAmo = 200,
            GroundInclinationAngle = 1000,
        }

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (StatusType* st = &statusType)
            fixed (ComparisonType* ct = &comparisonType)
            fixed (ComparatorType* at = &assessmentType)
            fixed (SpeedUnitType* sut = &speedUnitType)
            fixed (int* w = &weapon)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_assessSelfStatusFuncPar.status, pgNodeParDescription_assessSelfStatusFuncPar.status);
                pgbepManager.SetPgbepEnum(typeof(StatusType), (int*)st);
                if (statusType is StatusType.WeaponAmo)
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_assessSelfStatusFuncPar.weapon, pgNodeParDescription_assessSelfStatusFuncPar.weapon);
                    pgbepManager.SetPgbepSelectOptions(w, pgbepManager.GetEquipmentList());
                }
                if (statusType is StatusType.OptionalPartsAmo)
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_assessSelfStatusFuncPar.optionalParts, pgNodeParDescription_assessSelfStatusFuncPar.optionalParts);
                    pgbepManager.SetPgbepSelectOptions(w, pgbepManager.GetOptionalList());
                }
                if (statusType is not StatusType.Speed)
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_assessSelfStatusFuncPar.comparisonType, pgNodeParDescription_assessSelfStatusFuncPar.comparisonType);
                    pgbepManager.SetPgbepEnum(typeof(ComparisonType), (int*)ct);
                }
                else
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_assessSelfStatusFuncPar.speedUnit, pgNodeParDescription_assessSelfStatusFuncPar.speedUnit);
                    pgbepManager.SetPgbepEnum(typeof(SpeedUnitType), (int*)sut);
                }
                pgbepManager.SetHeaderText(pgNodeParameter_assessSelfStatusFuncPar.value, pgNodeParDescription_assessSelfStatusFuncPar.value);
                assessmentValueV.IndicateSwitchableFloat(pgbepManager, comparisonType is ComparisonType.Percentage ? new PgbepManager.FloatSliderSettingPar(0, 100) : null);
                pgbepManager.SetHeaderText(pgNodeParameter_assessSelfStatusFuncPar.@operator, pgNodeParDescription_assessSelfStatusFuncPar.@operator);
                pgbepManager.SetPgbepEnum(typeof(ComparatorType), (int*)at);
            }
        }

        public override bool BranchExecute(MachineLD ld)
        {
            float nowPar;
            switch (statusType)
            {
                case StatusType.HealthPoint:
                    nowPar = ld.cd.maxHearthPoint - ld.statePar.damage;
                    break;
                case StatusType.Speed:
                    nowPar = ld.movePar.rBody.linearVelocity.magnitude * speedUnitType.GetSpeedUnitRatio();
                    break;
                case StatusType.Heat:
                    nowPar = ld.statePar.heat;
                    break;
                case StatusType.Energy:
                    nowPar = ld.powerPlantData.energyCapacity - ld.statePar.energyUsed;
                    break;
                case StatusType.Shield:
                    nowPar = ld.shieldCd.healthPoint - ld.statePar.shieldDamage;
                    break;
                case StatusType.Impact:
                    nowPar = ld.statePar.impact;
                    break;
                case StatusType.WeaponAmo:
                    var amoNum = ld.customData.mechCustom.weaponAmoNum[weapon];
                    var usedNum = ld.runningShootHolder[weapon].numberOfShots;
                    nowPar = amoNum - usedNum;
                    break;
                case StatusType.OptionalPartsAmo:
                    var opNum = ld.customData.mechCustom.optionPartsUsableNum[weapon];
                    ld.statePar.optionPartsUseCount.TryGetValue(weapon, out var usedNum2);
                    nowPar = opNum - usedNum2;
                    break;
                case StatusType.GroundInclinationAngle:
                    nowPar = Vector3.Angle(ld.movePar.groundNormal, Vector3.up);
                    break;
                default:
                    return false;
            }

            float av = assessmentValueV.GetUseValueFloat(ld);
            switch (comparisonType, statusType)
            {
                case (ComparisonType.RealNumber, _):
                case (_, StatusType.Speed):
                case (_, StatusType.GroundInclinationAngle):
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
                        case StatusType.WeaponAmo:
                            var amoNum = ld.customData.mechCustom.weaponAmoNum[weapon];
                            av = av / 100 * amoNum;
                            break;
                        case StatusType.OptionalPartsAmo:
                            var opNum = ld.customData.mechCustom.optionPartsUsableNum[weapon];
                            av = av / 100 * opNum;
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
            return new[] { $"{statusType} {comparatorStr} {assessmentValueV.GetIndicateStr()}{comparisonStr}" };
        }
    }
}