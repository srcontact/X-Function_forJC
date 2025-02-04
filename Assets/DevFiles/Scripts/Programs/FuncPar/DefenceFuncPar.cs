using static I2.Loc.ScriptLocalization;
using System;
using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.ClAction.Machines.Motion;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using MemoryPack;
using static clrev01.ClAction.Machines.CharMoveState;
using static clrev01.ClAction.Machines.Motion.MotionMoveType;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class DefenceFuncPar : ActionFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.defence;
        public DefenceActionType actionNum;

        private MachineHD nowEditHd => MHUB.GetData(StaticInfo.Inst.nowEditMech.mechCustom.machineCode).machineCD.origHD;

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            //todo:ノードエディタに警告などのテキスト表示を実装する
            if (actionNum is DefenceActionType.Guard && !nowEditHd.guardActionAvailable)
            {
                pgbepManager.SetHeaderText(pgNodeWarning_defenceFuncPar.guard_operation_is_not_possible_for_this_model);
            }
            if (actionNum is DefenceActionType.Cover && !nowEditHd.coverActionAvailable)
            {
                pgbepManager.SetHeaderText(pgNodeWarning_defenceFuncPar.cover_operation_is_not_possible_for_this_model);
            }
            fixed (DefenceActionType* an = &actionNum)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_defenceFuncPar.defenceType, pgNodeParDescription_defenceFuncPar.defenceType);
                pgbepManager.SetPgbepEnum(typeof(DefenceActionType), (int*)an);
            }
        }
        public override bool ActionExecute(MachineLD ld)
        {
            if (!DefenceExecutable(ld.hd)) return true;

            ld.movePar.ActBreak(ld, actionNum switch
            {
                DefenceActionType.Guard => ld.cd.MoveCommonPar.guardBreakRatio,
                DefenceActionType.Cover => ld.cd.MoveCommonPar.coverBreakRatio,
                _ => throw new ArgumentOutOfRangeException()
            });
            return false;
        }
        private bool DefenceExecutable(MachineHD hd)
        {
            return (actionNum is DefenceActionType.Guard && hd.guardActionAvailable) ||
                   (actionNum is DefenceActionType.Cover && hd.coverActionAvailable);
        }
        public override bool CheckIsExecutable(MachineLD ld)
        {
            return
                ld.movePar.moveState == isGrounded &&
                DefenceExecutable(ld.hd) &&
                (!ld.DuringLandingRigidity ||
                 (ld.actionState is ActionState.Guard && actionNum is DefenceActionType.Guard) ||
                 (ld.actionState is ActionState.Cover && actionNum is DefenceActionType.Cover)
                );
        }
        public override bool ActionOverwritable(ActionFuncPar other, MachineLD ld)
        {
            if (!DefenceExecutable(ld.hd)) return false;
            if (ld.movePar.moveState is not isGrounded) return false;
            return other switch
            {
                MoveTypeFuncPar => true,
                StopActionFuncPar x => x.stopActionType is StopActionType.All or StopActionType.Defence,
                FireFuncPar => true,
                FightFuncPar => ld.movePar.moveState == isGrounded,
                DefenceFuncPar x => x.actionNum != actionNum && ld.movePar.moveState == isGrounded,
                _ => false
            };
        }
        public override float EnergyCost(MachineLD ld)
        {
            return actionNum switch
            {
                DefenceActionType.Guard => ld.cd.MoveCommonPar.guardUseEnergy,
                DefenceActionType.Cover => ld.cd.MoveCommonPar.coverUseEnergy,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        public override float HeatCost(MachineLD ld)
        {
            return 0;
        }
    }
}