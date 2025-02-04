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
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class ThrustFuncPar : ActionFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.thrust;
        public DirectionSettingType directionSettingType;
        public CoordinateSystemType coordinateSystemType;
        public VariableDataNumericGet horizontalAngleV = new();
        public VariableDataNumericGet verticalAngleV = new() { constValue = 45 };
        public VariableDataVector3Get vectorV = new();
        public VariableDataNumericGet powerV = new() { constValue = 100 };
        public ThrustContinuationType cType;
        public VariableDataNumericGet cParV = new() { constValue = 5 };
        public ThrustMode thrustMode;
        private int _endFrame;

        public enum ThrustContinuationType
        {
            Second,
            Frame,
        }


        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (DirectionSettingType* dst = &directionSettingType)
            fixed (CoordinateSystemType* cst = &coordinateSystemType)
            fixed (ThrustContinuationType* ct = &cType)
            fixed (ThrustMode* tm = &thrustMode)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_thrustFuncPar.thrustMode, pgNodeParDescription_thrustFuncPar.thrustMode);
                pgbepManager.SetPgbepEnum(typeof(ThrustMode), (int*)tm);
                pgbepManager.SetHeaderText(pgNodeParameter_thrustFuncPar.direction, pgNodeParDescription_thrustFuncPar.direction);
                pgbepManager.SetPgbepEnum(typeof(DirectionSettingType), (int*)dst);
                pgbepManager.SetPgbepEnum(typeof(CoordinateSystemType), (int*)cst);
                switch (directionSettingType)
                {
                    case DirectionSettingType.Angles:
                        pgbepManager.SetPgbepAngle(hAngleLimit: null, vAngleLimit: null, hVd: horizontalAngleV, vVd: verticalAngleV);
                        break;
                    case DirectionSettingType.Vector3D:
                        vectorV.IndicateSwitchable(pgbepManager);
                        break;
                }
                pgbepManager.SetHeaderText(pgNodeParameter_thrustFuncPar.power, pgNodeParDescription_thrustFuncPar.power);
                powerV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(0, 100));
                if (thrustMode is ThrustMode.Normal)
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_thrustFuncPar.continuation, pgNodeParDescription_thrustFuncPar.continuation);
                    pgbepManager.SetPgbepEnum(typeof(ThrustContinuationType), (int*)ct);
                    float max;
                    int unit;
                    switch (cType)
                    {
                        case ThrustContinuationType.Frame:
                            max = 120;
                            unit = 0;
                            break;
                        case ThrustContinuationType.Second:
                        default:
                            max = 120;
                            unit = 2;
                            break;
                    }
                    cParV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(1, max, unit));
                }
            }
        }

        public override bool CheckIsExecutable(MachineLD ld)
        {
            return thrustMode is ThrustMode.Normal || ld.runningThrustHolder.RunningAction == this || (ld.statePar.quickThrustUseableFrame <= ACM.actionFrame);
        }

        public override void OnChangeAction(MachineLD ld)
        {
            base.OnChangeAction(ld);
            if (thrustMode is ThrustMode.Quick)
            {
                _endFrame = ACM.actionFrame + ld.thrusterData.quickThrustFrame;
                ld.statePar.quickThrustUseableFrame = _endFrame + ld.thrusterData.quickThrustInterval;
                Debug.Log($"end frame {_endFrame} / quickThrustUseableFrame {ld.statePar.quickThrustUseableFrame}");
            }
        }

        public override void InitOnExecute(MachineLD ld)
        {
            base.InitOnExecute(ld);
            if (thrustMode == ThrustMode.Normal)
            {
                _endFrame = ActionManager.Inst.actionFrame;
                var cp = cParV.GetUseValueInt(ld);
                switch (cType)
                {
                    case ThrustContinuationType.Second:
                    default:
                        _endFrame += (int)(cp * 60);
                        break;
                    case ThrustContinuationType.Frame:
                        _endFrame += (int)cp;
                        break;
                }
            }
        }

        public override bool ActionExecute(MachineLD ld)
        {
            Vector3 v;
            switch (directionSettingType)
            {
                case DirectionSettingType.Angles:
                default:
                    var hv = Mathf.Deg2Rad * horizontalAngleV.GetUseValueFloat(ld);
                    var vv = Mathf.Deg2Rad * verticalAngleV.GetUseValueFloat(ld);
                    v = new Vector3(
                        Mathf.Sin(hv) * Mathf.Cos(vv),
                        Mathf.Sin(vv),
                        Mathf.Cos(hv) * Mathf.Cos(vv));
                    break;
                case DirectionSettingType.Vector3D:
                    v = vectorV.GetUseValue(ld);
                    break;
            }
            if (coordinateSystemType is CoordinateSystemType.Global) v = ld.hd.transform.InverseTransformVector(v);
            ld.movePar.ActThrust(thrustMode, v.normalized * powerV.GetUseValueFloat(ld, (float?)0, 100) / 100, ld.thrusterData);
            ld.statePar.heat += HeatCost(ld);
            return false;
        }

        public override bool CheckEnd(MachineLD ld)
        {
            return _endFrame <= ActionManager.Inst.actionFrame;
        }

        public override bool ActionOverwritable(ActionFuncPar other, MachineLD ld)
        {
            return other switch
            {
                ThrustFuncPar => thrustMode is not ThrustMode.Quick,
                StopActionFuncPar x => x.stopActionType is StopActionType.All or StopActionType.Thrust,
                _ => false
            };
        }
        public override float EnergyCost(MachineLD ld)
        {
            float thrustUseEnergy = thrustMode is ThrustMode.Normal ? ld.thrusterData.thrustUseEnergy : ld.thrusterData.quickThrustUseEnergy / ld.thrusterData.quickThrustFrame;
            return thrustUseEnergy * powerV.GetUseValueFloat(ld, (float?)0, 100) / 100;
        }

        public override float HeatCost(MachineLD ld)
        {
            float thrustHeat = thrustMode is ThrustMode.Normal ? ld.thrusterData.thrustHeat : ld.thrusterData.quickThrustHeat / ld.thrusterData.quickThrustFrame;
            return thrustHeat * powerV.GetUseValueFloat(ld, (float?)0, 100) / 100;
        }
        public override float?[] GetNodeFaceValue()
        {
            var h = directionSettingType switch
            {
                DirectionSettingType.Angles => horizontalAngleV.useVariable ? 0f : horizontalAngleV.constValue,
                DirectionSettingType.Vector3D => vectorV.useVariable ? 0 : CalcHorizontalAngle(vectorV.constValue),
                _ => throw new ArgumentOutOfRangeException()
            };
            var v = 90 - directionSettingType switch
            {
                DirectionSettingType.Angles => (verticalAngleV.useVariable ? 0f : verticalAngleV.constValue),
                DirectionSettingType.Vector3D => vectorV.useVariable ? 0 : CalcVerticalAngle(vectorV.constValue),
                _ => throw new ArgumentOutOfRangeException()
            };
            return new float?[]
            {
                h,
                powerV.useVariable ? null : powerV.constValue / 100f,
                thrustMode is ThrustMode.Normal && cParV.useVariable ? null : cParV.constValue / 120f,
                v
            };
        }
        public override string[] GetNodeFaceText()
        {
            var continuationText = cParV.GetIndicateStr((cType, thrustMode) switch
            {
                (_, ThrustMode.Quick) => null,
                (ThrustContinuationType.Second, _) => $"sec",
                (ThrustContinuationType.Frame, _) => $"frm",
                _ => null
            });
            var v = directionSettingType switch
            {
                DirectionSettingType.Angles => verticalAngleV.GetIndicateStr("°"),
                DirectionSettingType.Vector3D => null,
                _ => throw new ArgumentOutOfRangeException()
            };
            var r = directionSettingType switch
            {
                DirectionSettingType.Angles => $"{horizontalAngleV.GetIndicateStr()}°",
                DirectionSettingType.Vector3D => vectorV.GetIndicateStr(),
                _ => throw new ArgumentOutOfRangeException()
            };
            return new[] { powerV.GetIndicateStr("%"), continuationText, v, r };
        }
    }
}