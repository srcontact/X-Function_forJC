using static I2.Loc.ScriptLocalization;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save.VariableData;
using MemoryPack;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class StopProgramFuncPar : FunctionFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.stopProgram;
        public override int calcCost => 0;
        public VariableDataNumericGet durationV = new() { constValue = 1 };

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            pgbepManager.SetHeaderText(pgNodeParameter_stopProgramFuncPar.duration, pgNodeParDescription_stopProgramFuncPar.duration);
            durationV.IndicateSwitchableInt(pgbepManager, new PgbepManager.IntSliderSettingPar(1, 120, true, false));
        }

        public override float?[] GetNodeFaceValue()
        {
            return new[] { 0, null, durationV.GetGaugeValue() / 120f, null };
        }
        public override string[] GetNodeFaceText()
        {
            return new[] { null, durationV.GetIndicateStr("frm"), null };
        }
    }
}