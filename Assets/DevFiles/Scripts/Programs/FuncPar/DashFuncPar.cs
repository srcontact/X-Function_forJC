using static I2.Loc.ScriptLocalization;
using clrev01.ClAction;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class DashFuncPar : MoveTypeFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.dash;
        protected override float? iconRotateValue =>
            directionSettingType switch
            {
                DirectionSettingType.Angles => horizontalAngleV.useVariable ? 0 : horizontalAngleV.constValue,
                DirectionSettingType.Vector3D => vectorV.useVariable ? 0 : CalcHorizontalAngle(vectorV.constValue),
                _ => throw new ArgumentOutOfRangeException()
            };
        protected override string iconRotateString =>
            directionSettingType switch
            {
                DirectionSettingType.Angles => $"{horizontalAngleV.GetIndicateStr()}°",
                DirectionSettingType.Vector3D => vectorV.GetIndicateStr(),
                _ => throw new ArgumentOutOfRangeException()
            };
        protected override float? powerGaugeValue => powerV.GetGaugeValue(0.01f);
        protected override string powerText => powerV.GetIndicateStr("%", 0.01f);
        protected override float? continuationGaugeValue => cParV.GetGaugeValue(1 / maxContinuationPar);
        protected override string continuationText =>
            cParV.GetIndicateStr(cType switch
            {
                ContinuationType.Second => $"sec",
                ContinuationType.Frame => $"frm",
                ContinuationType.Steps => $"stp",
                _ => null
            });

        public DirectionSettingType directionSettingType;
        public VariableDataNumericGet horizontalAngleV = new();
        public VariableDataVector3Get vectorV = new() { constValue = Vector3.forward };
        public VariableDataNumericGet powerV = new() { constValue = 100 };
        public ContinuationType cType;
        public VariableDataNumericGet cParV = new() { constValue = 5 };
        public CoordinateSystemType coordinateSystemType;
        private float maxContinuationPar =>
            cType switch
            {
                ContinuationType.Second => 120,
                ContinuationType.Frame => 120,
                ContinuationType.Steps => 100,
                _ => throw new ArgumentOutOfRangeException()
            };
        private int _endConditionValue;

        public enum ContinuationType
        {
            Second,
            Frame,
            Steps,
        }


        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (DirectionSettingType* dst = &directionSettingType)
            fixed (ContinuationType* ct = &cType)
            fixed (CoordinateSystemType* cst = &coordinateSystemType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_dashFuncPar.direction, pgNodeParDescription_dashFuncPar.direction);
                pgbepManager.SetPgbepEnum(typeof(DirectionSettingType), (int*)dst);
                pgbepManager.SetPgbepEnum(typeof(CoordinateSystemType), (int*)cst);
                switch (directionSettingType)
                {
                    case DirectionSettingType.Angles:
                        pgbepManager.SetPgbepAngle(hAngleLimit: null, vAngleLimit: null, hVd: horizontalAngleV, vVd: null);
                        break;
                    case DirectionSettingType.Vector3D:
                        vectorV.IndicateSwitchable(pgbepManager, new[] { true, false, true });
                        break;
                }
                pgbepManager.SetHeaderText(pgNodeParameter_dashFuncPar.power, pgNodeParDescription_dashFuncPar.power);
                powerV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(0, 100));
                pgbepManager.SetHeaderText(pgNodeParameter_dashFuncPar.continuation, pgNodeParDescription_dashFuncPar.continuation);
                pgbepManager.SetPgbepEnum(typeof(ContinuationType), (int*)ct);
                int unit;
                switch (cType)
                {
                    case ContinuationType.Frame:
                        unit = 0;
                        break;
                    case ContinuationType.Second:
                    default:
                        unit = 2;
                        break;
                    case ContinuationType.Steps:
                        unit = 0;
                        break;
                }
                cParV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(1, maxContinuationPar, unit));
            }
        }

        public override bool CheckChangeAction(MachineLD ld)
        {
            return Vector3.Dot(MoveDirection(ld).normalized, ld.hd.legMover.LatestMoveDirection.normalized) < 0;
        }

        public override void OnChangeAction(MachineLD ld)
        {
            base.OnChangeAction(ld);
            ld.hd.legMover.Landing(ld.hd.rigidBody.linearVelocity);
            ld.hd.legMover.resetStartPosFlag = true;
            ld.hd.legMover.SetLegStartPos();
        }

        public override void InitOnExecute(MachineLD ld)
        {
            base.InitOnExecute(ld);
            var cp = cParV.GetUseValueInt(ld);
            switch (cType)
            {
                case ContinuationType.Second:
                default:
                    _endConditionValue = ActionManager.Inst.actionFrame + (int)(cp * 60);
                    break;
                case ContinuationType.Frame:
                    _endConditionValue = ActionManager.Inst.actionFrame + (int)cp;
                    break;
                case ContinuationType.Steps:
                    _endConditionValue = ld.hd.legMover.stepCount + (int)cp;
                    break;
            }
        }

        public override bool ActionExecute(MachineLD ld)
        {
            if (ld.movePar.moveState != CharMoveState.isGrounded) return true;
            if (!ld.hd.legMover.legsGrounded) return false;
            var vector = ld.hd.transform.InverseTransformVector(MoveDirection(ld));
            var pow = powerV.GetUseValueFloat(ld, 0, 100) / 100;
            ld.movePar.ActDash(vector, pow);
            return false;
        }

        public override Vector3 MoveDirection(MachineLD ld)
        {
            Vector3 v;
            switch (directionSettingType)
            {
                case DirectionSettingType.Angles:
                    var ha = Mathf.Deg2Rad * horizontalAngleV.GetUseValueFloat(ld);
                    v = new Vector3(
                        Mathf.Sin(ha),
                        0,
                        Mathf.Cos(ha));
                    break;
                case DirectionSettingType.Vector3D:
                    v = vectorV.GetUseValue(ld);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (coordinateSystemType is CoordinateSystemType.Local) v = ld.hd.transform.TransformVector(v);
            v = v.normalized;
            return v;
        }

        public override bool CheckEnd(MachineLD ld)
        {
            switch (cType)
            {
                case ContinuationType.Frame:
                case ContinuationType.Second:
                default:
                    return _endConditionValue < ActionManager.Inst.actionFrame;
                case ContinuationType.Steps:
                    return _endConditionValue < ld.hd.legMover.stepCount;
            }
        }

        public override float EnergyCost(MachineLD ld)
        {
            return ld.cd.MoveCommonPar.dashUseEnergy * powerV.GetUseValueFloat(ld, (float?)0, 100) / 100;
        }

        public override float HeatCost(MachineLD ld)
        {
            return 0;
        }

        public override float GetPowerValue(MachineLD ld)
        {
            return powerV.GetUseValueFloat(ld, (float?)0, 100) / 100;
        }
        public override bool IsActiveDownforce => true;
    }
}