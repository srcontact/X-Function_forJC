using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.ClAction.Machines.Motion;
using MemoryPack;
using UnityEngine;

namespace clrev01.Programs.FuncPar.FuncParType
{
    [System.Serializable]
    public abstract class MoveTypeFuncPar : ActionFuncPar
    {
        public override bool ActionOverwritable(ActionFuncPar other, MachineLD ld)
        {
            return other switch
            {
                MoveTypeFuncPar => true,
                StopActionFuncPar s => s.stopActionType is StopActionType.All or StopActionType.Move,
                FightFuncPar f => ld.hd.fightMover.fightMotionData[f.fightActionNum].motionMoveType is not MotionMoveType.none && f.enableMove,
                DefenceFuncPar f => ld.movePar.moveState is CharMoveState.isGrounded,
                _ => false
            };
        }

        public override void OnChangeAction(MachineLD ld)
        {
            base.OnChangeAction(ld);
            ld.hd.legMover.exeCount = 0;
            ld.hd.legMover.AdvanceWalkCycle();
        }

        public override bool CheckIsExecutable(MachineLD ld)
        {
            return ld.movePar.moveState == CharMoveState.isGrounded && !ld.DuringLandingRigidity && ld.statePar.impact <= 0;
        }

        public virtual Vector3 MoveDirection(MachineLD ld)
        {
            return Vector3.up;
        }

        protected virtual float? iconRotateValue => null;
        protected virtual string iconRotateString => null;
        protected virtual float? powerGaugeValue => null;
        protected virtual string powerText => null;
        protected virtual float? continuationGaugeValue => null;
        protected virtual string continuationText => null;
        protected virtual float? verticalRotateValue => null;
        protected virtual string verticalRotateText => null;
        public override float?[] GetNodeFaceValue()
        {
            return new[] { iconRotateValue, powerGaugeValue, continuationGaugeValue, verticalRotateValue };
        }
        public override string[] GetNodeFaceText()
        {
            return new[] { powerText, continuationText, verticalRotateText, iconRotateString };
        }
        public abstract float GetPowerValue(MachineLD ld);

        [MemoryPackIgnore]
        public abstract bool IsActiveDownforce { get; }
    }
}