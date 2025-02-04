using static I2.Loc.ScriptLocalization;
using clrev01.ClAction.Machines;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using MemoryPack;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class StopActionFuncPar : ActionFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.stopAction;

        public StopActionType stopActionType;
        public long weaponFlags = long.MaxValue;


        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            fixed (StopActionType* t = &stopActionType)
            fixed (long* w = &weaponFlags)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_stopActionFuncPar.stopType, pgNodeParDescription_stopActionFuncPar.stopType);
                pgbepManager.SetPgbepEnum(typeof(StopActionType), (int*)t);
                if (stopActionType == StopActionType.Fire)
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_stopActionFuncPar.weaponNumber, pgNodeParDescription_stopActionFuncPar.weaponNumber);
                    pgbepManager.SetPgbepSelectFlagsOption(w, pgbepManager.GetEquipmentList());
                }
            }
        }

        public override bool ActionExecute(MachineLD ld)
        {
            return true;
        }

        public override bool CheckEnd(MachineLD ld)
        {
            return true;
        }

        public override bool ActionOverwritable(ActionFuncPar other, MachineLD ld)
        {
            return stopActionType switch
            {
                StopActionType.Move => other is MoveTypeFuncPar,
                StopActionType.Rotate => other is RotateFuncPar,
                StopActionType.Fire => other is FireFuncPar,
                _ => true
            };
        }

        public override float EnergyCost(MachineLD ld)
        {
            return 0;
        }

        public override float HeatCost(MachineLD ld)
        {
            return 0;
        }

        public override StopActionType? GetNodeFaceStopActionType()
        {
            return stopActionType;
        }
        public override long? GetNodeFaceSelectedWeapons()
        {
            return stopActionType is StopActionType.Fire ? weaponFlags : null;
        }
    }
}