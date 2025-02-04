using static I2.Loc.ScriptLocalization;
using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using MemoryPack;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Programs.FuncPar
{
    [MemoryPackable]
    public partial class UseOptionalFuncPar : FunctionFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.useOptional;
        public int useSlot;

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (int* us = &useSlot)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_optionalPartsUseFuncPar.use_Slot, pgNodeParDescription_optionalPartsUseFuncPar.use_Slot);
                pgbepManager.SetPgbepSelectOptions(us, pgbepManager.GetOptionalList());
            }
        }
        public void ExeUseOptional(MachineLD ld)
        {
            var op = OpHub.GetOptionPartsData(ld.customData.mechCustom.optionParts[useSlot]);
            ld.statePar.optionPartsUseCount.TryGetValue(useSlot, out var useNum);
            var usableNum = ld.customData.mechCustom.optionPartsUsableNum[useSlot];
            if (useNum >= usableNum) return;

            int usingFrame = int.MinValue;
            if (ld.statePar.optionPartsUseFrameDict.ContainsKey(useSlot))
            {
                ld.statePar.optionPartsUseFrameDict.TryGetValue(useSlot, out usingFrame);
            }
            if (usingFrame < ACM.actionFrame)
            {
                ld.statePar.optionPartsUseCount[useSlot] = useNum + 1;
                ld.statePar.optionPartsUseFrameDict[useSlot] = ACM.actionFrame + op.data.durationFrame;
                op.data.InitOptionParts(ld, useSlot);
            }
        }
    }
}