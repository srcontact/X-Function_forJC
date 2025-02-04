using static I2.Loc.ScriptLocalization;
using clrev01.Bases;
using clrev01.ClAction;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FieldPar;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using System;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class ShieldFuncPar : ActionFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.shield;
        public ShieldFieldPar fieldPar = new();
        public ShieldContinuationType continuationType;
        public VariableDataNumericGet continuationParV = new() { constValue = 5 };

        private int _endFrame;

        public enum ShieldContinuationType
        {
            Second,
            Frame,
        }

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (ShieldContinuationType* ct = &continuationType)
            fixed (CoordinateSystemType* cst = &fieldPar.coordinateSystemType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_shieldFuncPar.coordinate_System, pgNodeParDescription_shieldFuncPar.coordinate_System);
                pgbepManager.SetPgbepEnum(typeof(CoordinateSystemType), (int*)cst);
                pgbepManager.SetHeaderText(pgNodeParameter_shieldFuncPar.shield_Parameter, pgNodeParDescription_shieldFuncPar.shield_Parameter);
                pgbepManager.SetPgbepField(fieldPar, (res) => { fieldPar = (ShieldFieldPar)res; });
                pgbepManager.SetHeaderText(pgNodeParameter_shieldFuncPar.continuation, pgNodeParDescription_shieldFuncPar.continuation);
                pgbepManager.SetPgbepEnum(typeof(ShieldContinuationType), (int*)ct);
                switch (continuationType)
                {
                    case ShieldContinuationType.Second:
                        continuationParV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(0, 10, 2));
                        break;
                    case ShieldContinuationType.Frame:
                        continuationParV.IndicateSwitchableInt(pgbepManager, new PgbepManager.IntSliderSettingPar(1, 120));
                        break;
                }
            }
        }
        public override void InitOnExecute(MachineLD ld)
        {
            base.InitOnExecute(ld);
            var continuationPar = continuationParV.GetUseValueFloat(ld);
            switch (continuationType)
            {
                case ShieldContinuationType.Second:
                default:
                    _endFrame = ActionManager.Inst.actionFrame + (int)(continuationPar * 60);
                    break;
                case ShieldContinuationType.Frame:
                    _endFrame = ActionManager.Inst.actionFrame + (int)continuationPar;
                    break;
            }
        }
        public override bool ActionExecute(MachineLD ld)
        {
            if (ld.shieldCd == null || ld.hd.shieldHd == null) return true;
            var minMax = ld.shieldCd.GetShieldMinMax(fieldPar.coordinateSystemType);
            var radius = fieldPar.radiusV.GetUseValueFloat(ld, minMax.radiusMin, minMax.radiusMax);
            if (ld.statePar.shieldBreakFlag || ld.statePar.shieldDamage >= ld.shieldCd.healthPoint)
            {
                ld.hd.shieldHd.ShieldSetting(false, radius, ld.hd.pos, false);
                return true;
            }
            if (!ld.hd.shieldHd.shieldActive && ld.statePar.latestShieldUseFrame + ld.shieldCd.reloadFrame >= ACM.actionFrame)
            {
                return true;
            }
            if (!ld.hd.shieldHd.shieldActive)
            {
                ld.statePar.latestShieldStartFrame = ACM.actionFrame;
            }
            var center = fieldPar.offsetV.GetUseValue(ld);
            Vector3 shieldCenter = fieldPar.coordinateSystemType switch
            {
                CoordinateSystemType.Local => ld.hd.transform.TransformPoint(center),
                CoordinateSystemType.Global => center,
                _ => throw new ArgumentOutOfRangeException(nameof(fieldPar.coordinateSystemType))
            };
            var toCenterV = shieldCenter - ld.hd.pos;
            if (toCenterV.sqrMagnitude > ld.shieldCd.maxCenterRange * ld.shieldCd.maxCenterRange)
            {
                shieldCenter = ld.hd.pos + toCenterV.normalized * ld.shieldCd.maxCenterRange;
            }
            ld.hd.shieldHd.ShieldSetting(true, radius, shieldCenter, fieldPar.coordinateSystemType is CoordinateSystemType.Local);
            ld.statePar.shieldDamage += ld.shieldCd.useDamageRateParFrame;
            ld.statePar.latestShieldUseFrame = ACM.actionFrame;
            return false;
        }
        public override bool CheckEnd(MachineLD ld)
        {
            return _endFrame < ActionManager.Inst.actionFrame;
        }
        public override void OnEndAction(MachineLD ld)
        {
            base.OnEndAction(ld);
            if (ld.hd.shieldHd != null) ld.hd.shieldHd.ShieldSetting(false, 0, Vector3.zero, false);
        }
        public override bool ActionOverwritable(ActionFuncPar other, MachineLD ld)
        {
            return other switch
            {
                ShieldFuncPar => true,
                StopActionFuncPar x => x.stopActionType is StopActionType.All or StopActionType.Shield,
                _ => false
            };
        }
        public override float EnergyCost(MachineLD ld)
        {
            if (ld.shieldCd == null) return 0;
            var minMax = ld.shieldCd.GetShieldMinMax(fieldPar.coordinateSystemType);
            return ld.shieldCd.useEnergy * fieldPar.radiusV.GetUseValueFloat(ld, minMax.radiusMin, minMax.radiusMax);
        }
        public override float HeatCost(MachineLD ld)
        {
            return 0;
        }
        public override string[] GetNodeFaceText()
        {
            return new[] { fieldPar.GetFieldShortText() };
        }
        public override IFieldEditObject GetNodeFaceIFieldEditObject()
        {
            return fieldPar;
        }
    }
}