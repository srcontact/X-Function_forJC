using static I2.Loc.ScriptLocalization;
using clrev01.ClAction;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save.VariableData;
using MemoryPack;
using UnityEngine;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class RotateFuncPar : ActionFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.rotate;
        public VariableDataNumericGet powV = new() { constValue = 100 };
        public RotateContinuationType cType;
        public VariableDataNumericGet cParV = new() { constValue = 5 };
        private int _endFrame;

        public enum RotateContinuationType
        {
            Second,
            Frame,
        }


        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (RotateContinuationType* ct = &cType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_rotateFuncPar.power, pgNodeParDescription_rotateFuncPar.power);
                powV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(-100, 100));
                pgbepManager.SetHeaderText(pgNodeParameter_rotateFuncPar.continuation, pgNodeParDescription_rotateFuncPar.continuation);
                pgbepManager.SetPgbepEnum(typeof(RotateContinuationType), (int*)ct);
                float min, max;
                int unit;
                switch (cType)
                {
                    case RotateContinuationType.Frame:
                        min = 1;
                        max = 120;
                        unit = 0;
                        break;
                    case RotateContinuationType.Second:
                    default:
                        min = 0;
                        max = 120;
                        unit = 2;
                        break;
                }
                cParV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(min, max, unit));
            }
        }

        public override bool CheckIsExecutable(MachineLD ld)
        {
            return ld.statePar.impact <= 0;
        }

        public override void InitOnExecute(MachineLD ld)
        {
            base.InitOnExecute(ld);
            var cp = cParV.GetUseValueInt(ld);
            switch (cType)
            {
                case RotateContinuationType.Second:
                default:
                    _endFrame = ActionManager.Inst.actionFrame + cp * 60;
                    break;
                case RotateContinuationType.Frame:
                    _endFrame = ActionManager.Inst.actionFrame + cp;
                    break;
            }
        }

        public override bool ActionExecute(MachineLD ld)
        {
            ld.movePar.ActRotateInAcceleAndLimit(powV.GetUseValueFloat(ld, -100, 100) / 100);
            return false;
        }

        public override bool CheckEnd(MachineLD ld)
        {
            return _endFrame < ActionManager.Inst.actionFrame;
        }

        public override bool ActionOverwritable(ActionFuncPar other, MachineLD ld)
        {
            return other switch
            {
                RotateFuncPar => true,
                StopActionFuncPar s => s.stopActionType is StopActionType.All or StopActionType.Rotate,
                _ => false
            };
        }

        public override float EnergyCost(MachineLD ld)
        {
            return ld.cd.MoveCommonPar.rotateUseEnergy * Mathf.Abs(powV.GetUseValueFloat(ld, -100, 100)) / 100;
        }

        public override float HeatCost(MachineLD ld)
        {
            return 0;
        }

        public override float?[] GetNodeFaceValue()
        {
            return new[]
            {
                powV.useVariable ? 100f : powV.constValue, powV.GetGaugeValue(0.01f, true), cParV.GetGaugeValue(0.01f), null
            };
        }
        public override string[] GetNodeFaceText()
        {
            return new[]
            {
                powV.GetIndicateStr("%", 0.01f),
                cParV.GetIndicateStr(cType switch
                {
                    RotateContinuationType.Second => $"sec",
                    RotateContinuationType.Frame => $"frm",
                    _ => null
                }),
                null
            };
        }
    }
}