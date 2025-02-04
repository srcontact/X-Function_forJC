using static I2.Loc.ScriptLocalization;
using clrev01.ClAction;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using clrev01.Save.VariableData;
using MemoryPack;
using System;

namespace clrev01.Programs.FuncPar
{
    [Serializable]
    [MemoryPackable()]
    public partial class GetBattleStatusValueFuncPar : FunctionFuncPar, IPGBFuncUnion, IGetStatusValueFuncPar
    {
        public override string BlockTypeStr => pgNodeName.getBattleStatus;
        public BattleStatusValueType statusType;
        public VariableDataNumericSet tgtVn = new() { };

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (BattleStatusValueType* st = &statusType)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_getBattleStatusValueFuncPar.statusType, pgNodeParDescription_getBattleStatusValueFuncPar.statusType);
                pgbepManager.SetPgbepEnum(typeof(BattleStatusValueType), (int*)st);

                pgbepManager.SetHeaderText(pgNodeParameter_getBattleStatusValueFuncPar.targetVariable, pgNodeParDescription_getBattleStatusValueFuncPar.targetVariable);
                pgbepManager.SetPgbepVariable(tgtVn, false);
            }
        }

        public void GetStatusValue(MachineLD ld)
        {
            float res = 0;
            switch (statusType)
            {
                case BattleStatusValueType.TimeRemaining:
                    res = ActionManager.Inst.GetFrameRemaining / 60f;
                    break;
                case BattleStatusValueType.FrameRemaining:
                    res = ActionManager.Inst.GetFrameRemaining;
                    break;
                case BattleStatusValueType.ElapsedTime:
                    res = ActionManager.Inst.actionFrame / 60f;
                    break;
                case BattleStatusValueType.ElapsedFrame:
                    res = ActionManager.Inst.actionFrame;
                    break;
            }
            tgtVn.SetNumericValue(ld, res);
        }

        public override string[] GetNodeFaceText()
        {
            return new[] { $"ST:{statusType}\nTgtV:[{tgtVn.name}]" };
        }
    }
}