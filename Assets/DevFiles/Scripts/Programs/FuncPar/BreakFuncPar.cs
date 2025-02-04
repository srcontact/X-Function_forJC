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
    public partial class BreakFuncPar : MoveTypeFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.@break;
        protected override float? iconRotateValue => 0;
        protected override float? powerGaugeValue => powerV.GetGaugeValue(0.01f);
        protected override string powerText => powerV.GetIndicateStr("%", 0.01f);
        protected override float? continuationGaugeValue => continuationParV.GetGaugeValue(1 / maxContinuationPar);
        protected override string continuationText =>
            continuationParV.GetIndicateStr(continuationType switch
            {
                ContinuationType.Second => "sec",
                ContinuationType.Frame => "frm",
                _ => null
            });
        public VariableDataNumericGet powerV = new() { constValue = 100 };
        public ContinuationType continuationType;
        public VariableDataNumericGet continuationParV = new() { constValue = 1 };
        private float maxContinuationPar => 120;

        private int _endFrame;

        public enum ContinuationType
        {
            Second,
            Frame,
        }


        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (ContinuationType* ct = &continuationType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_breakFuncPar.power, pgNodeParDescription_breakFuncPar.power);
                powerV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(0, 100, 0));
                pgbepManager.SetHeaderText(pgNodeParameter_breakFuncPar.continuation, pgNodeParDescription_breakFuncPar.continuation);
                pgbepManager.SetPgbepEnum(typeof(ContinuationType), (int*)ct);
                continuationParV.IndicateSwitchableFloat(pgbepManager, new PgbepManager.FloatSliderSettingPar(1, maxContinuationPar, continuationType == ContinuationType.Frame ? 0 : 2));
            }
        }

        public override void InitOnExecute(MachineLD ld)
        {
            base.InitOnExecute(ld);
            _endFrame = ActionManager.Inst.actionFrame;
            var cp = continuationParV.GetUseValueInt(ld);
            switch (continuationType)
            {
                case ContinuationType.Second:
                default:
                    _endFrame += (int)(cp * 60);
                    break;
                case ContinuationType.Frame:
                    _endFrame += (int)cp;
                    break;
            }
        }

        public override bool ActionExecute(MachineLD ld)
        {
            ld.movePar.ActBreak(ld, powerV.GetUseValueFloat(ld, (float?)0, 100) / 100);
            return false;
        }

        public override bool CheckEnd(MachineLD ld)
        {
            return _endFrame < ActionManager.Inst.actionFrame;
        }

        public override float EnergyCost(MachineLD ld)
        {
            return ld.cd.MoveCommonPar.breakUseEnergy * powerV.GetUseValueFloat(ld, (float?)0, 100) / 100;
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

        public override Vector3 MoveDirection(MachineLD ld)
        {
            return -ld.hd.rigidBody.linearVelocity.normalized;
        }
    }
}