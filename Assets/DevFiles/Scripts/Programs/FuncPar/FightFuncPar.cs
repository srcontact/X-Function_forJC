using static I2.Loc.ScriptLocalization;
using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.ClAction.Machines.Motion;
using clrev01.ClAction.ObjectSearch;
using clrev01.Extensions;
using clrev01.PGE.PGBEditor;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save.VariableData;
using MemoryPack;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;
using static clrev01.ClAction.Machines.CharMoveState;
using static clrev01.ClAction.Machines.Motion.MotionMoveType;
using static clrev01.Extensions.ExPrediction;

namespace clrev01.Programs.FuncPar
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class FightFuncPar : ActionFuncPar, IPGBFuncUnion
    {
        public override string BlockTypeStr => pgNodeName.fight;
        public VariableDataLockOnGet targetList = new();
        public int fightActionNum;
        public bool enableHoming = true;
        public bool enableMove = true;
        public bool enableThruster = true;
        private MachineHD nowEditHd => MHUB.GetData(StaticInfo.Inst.nowEditMech.mechCustom.machineCode).machineCD.origHD;
        private int _executeFrame = 0;

        public override bool ActionOverwritable(ActionFuncPar other, MachineLD ld)
        {
            return other switch
            {
                MoveTypeFuncPar => ld.hd.fightMover.fightMotionData[fightActionNum].motionMoveType is jump,
                FireFuncPar => true,
                FightFuncPar x => x.fightActionNum != fightActionNum && ld.movePar.moveState == isGrounded,
                DefenceFuncPar => ld.movePar.moveState == isGrounded,
                _ => false
            };
        }

        public override unsafe void SetPointers(PgbepManager pgbepManager)
        {
            var fightList = nowEditHd.fightMover.fightMotionData;
            if (fightList.Count == 0)
            {
                pgbepManager.SetHeaderText(pgNodeWarning_fightFuncPar.this_machine_is_not_executable_Fighting);
                return;
            }
            fixed (int* fan = &fightActionNum)
            fixed (bool* eh = &enableHoming)
            fixed (bool* em = &enableMove)
            fixed (bool* et = &enableThruster)
            {
                pgbepManager.SetHeaderText(pgNodeParameter_fightFuncPar.target, pgNodeParDescription_fightFuncPar.target);
                targetList.IndicateWithIndex(pgbepManager);
                pgbepManager.SetHeaderText(pgNodeParameter_fightFuncPar.fightType, pgNodeParDescription_fightFuncPar.fightType);
                pgbepManager.SetPgbepSelectOptions(fan, fightList.ConvertAll(x => x.Name));
                pgbepManager.SetHeaderText(pgNodeParameter_fightFuncPar.enableHoming, pgNodeParDescription_fightFuncPar.enableHoming);
                pgbepManager.SetPgbepToggle(eh);
                if (fightList[fightActionNum].motionMoveType is not (none or jump))
                {
                    pgbepManager.SetHeaderText(pgNodeParameter_fightFuncPar.enableMove, pgNodeParDescription_fightFuncPar.enableMove);
                    pgbepManager.SetPgbepToggle(em);
                }
                pgbepManager.SetHeaderText(pgNodeParameter_fightFuncPar.enableThruster, pgNodeParDescription_fightFuncPar.enableThruster);
                pgbepManager.SetPgbepToggle(et);
                pgbepManager.SetSeparator();
            }
        }

        public override bool CheckIsExecutable(MachineLD ld)
        {
            return ld.movePar.moveState is not (recovery or uncontrollable) && (!ld.DuringLandingRigidity || ld.runningFightHolder.RunningAction != null);
        }

        public override void OnChangeAction(MachineLD ld)
        {
            base.OnChangeAction(ld);
            _executeFrame = 0;
            ld.hd.fightMover.OnStartFight(fightActionNum);
            if (CheckExecuteMoveAction(ld))
            {
                ld.hd.legMover.exeCount = 0;
                ld.hd.legMover.AdvanceWalkCycle();
                ld.hd.legMover.Landing(ld.hd.rigidBody.linearVelocity);
                ld.hd.legMover.resetStartPosFlag = true;
                ld.hd.legMover.SetLegStartPos();
            }
        }
        public override bool ActionExecute(MachineLD ld)
        {
            var nowMotionData = ld.hd.fightMover.nowMotionData;
            var fightHomingData = nowMotionData.homingData;
            var tgt = targetList.GetUseValue(ld);
            var checkHomingLange = enableHoming && CheckHomingLange(ld, tgt);
            ld.hd.fightMover.inHomingRange = checkHomingLange;
            Vector3 moveDirection;
            if (checkHomingLange)
            {
                if (ld.hd.fightMover.homingExe)
                {
                    var gravityAccele = ld.movePar.moveState == isGrounded ? 0 : ACM.actionEnvPar.globalGPowMSec;
                    var speed = ld.hd.rigidBody.linearVelocity.magnitude;
                    var cannotSpeedFixRatio =
                        1f - Mathf.Min(1f, (ld.movePar.moveState == isGrounded ? fightHomingData.groundedVelocityFixMaxSpeed : fightHomingData.inAirVelocityFixMaxSpeed) / speed);
                    Vector3 predictionPos = LinePrediction(
                        tgt.pos,
                        tgt.hardBase.rigidBody.linearVelocity,
                        speed,
                        0,
                        ld.hd.pos,
                        ld.hd.rigidBody.linearVelocity * cannotSpeedFixRatio,
                        gravityAccele
                    );
                    var tgtVector = predictionPos - ld.hd.pos;
                    ld.movePar.ActFightVelocityFix(tgtVector, fightHomingData);
                    moveDirection = ld.hd.transform.InverseTransformVector(tgtVector);
                }
                else
                {
                    moveDirection = ExMissileGuidance.GetProNavDirection(
                        tgt.pos,
                        ld.hd.rigidBody,
                        3,
                        1f / 60f,
                        ref ld.hd.fightMover.homingLosLog,
                        ld.hd.fightMover.previousHomingVector
                    );
                    ld.hd.fightMover.previousHomingVector = moveDirection;
                    moveDirection = ld.hd.transform.InverseTransformVector(moveDirection);
                }
            }
            else
            {
                moveDirection = Vector3.forward;
            }
            moveDirection = moveDirection.normalized;
            if (enableThruster && ld.hd.fightMover.nowMotionSegment.thrusterOn) ld.movePar.ActFightThrust(moveDirection, fightHomingData, ld.thrusterData);
            if ((enableMove || nowMotionData.motionMoveType is jump) && ld.runningMoveTypeHolder.RunningAction == null && CheckExecuteMoveAction(ld))
            {
                switch (nowMotionData.motionMoveType)
                {
                    case move:
                        ld.movePar.ActMove(moveDirection, 1);
                        break;
                    case dash:
                        if (!ld.hd.legMover.legsGrounded) ld.movePar.ActDash(moveDirection, 1);
                        break;
                    case jump:
                        if (_executeFrame == 0)
                        {
                            ld.movePar.ActJump(ld.hd.transform.TransformVector(moveDirection).normalized, 1, JumpFuncPar.JumpAngleReference.Global, nowMotionData.jumpMinVerticalAngle, ld.CalcWeightPar());
                        }
                        break;
                }
            }
            _executeFrame += 1;
            return false;
        }
        private bool CheckHomingLange(MachineLD ld, ObjectSearchTgt tgt)
        {
            if (tgt == null) return false;
            var nowMotionData = ld.hd.fightMover.nowMotionData;
            if (!nowMotionData.enableHoming) return false;
            var tgtPos = tgt.pos;
            var toTgtDiff = tgtPos - ld.hd.pos;
            if (toTgtDiff.magnitude > nowMotionData.homingData.homingStartLength) return false;
            var forward = ld.hd.transform.forward;
            var up = ld.hd.transform.up;
            var right = ld.hd.transform.right;
            var tgtHorizontalAngle = Vector3.SignedAngle(forward, Vector3.ProjectOnPlane(toTgtDiff, up), up);
            if (Mathf.Abs(tgtHorizontalAngle - nowMotionData.homingData.homingHorizontalAngleOffset) > nowMotionData.homingData.homingHorizontalAngle / 2) return false;
            var tgtVerticalAngle = Vector3.SignedAngle(forward, Vector3.ProjectOnPlane(toTgtDiff, right), right);
            if (Mathf.Abs(tgtVerticalAngle - nowMotionData.homingData.homingVerticalAngleOffset) > nowMotionData.homingData.homingVerticalAngle / 2) return false;
            return true;
        }
        public override bool CheckEnd(MachineLD ld)
        {
            return ld.hd.fightMover.checkEnd || ld.movePar.moveState is uncontrollable or recovery;
        }
        public override void OnEndAction(MachineLD ld)
        {
            base.OnEndAction(ld);
            ld.hd.fightMover.OnEndFight();
        }
        public override float EnergyCost(MachineLD ld)
        {
            return ld.hd.fightMover.nowMotionSegment.useEnergy;
        }
        public override float HeatCost(MachineLD ld)
        {
            return 0;
        }
        private bool CheckExecuteMoveAction(MachineLD ld)
        {
            return (enableMove || ld.hd.fightMover.nowMotionData.motionMoveType is jump) &&
                   ld.hd.fightMover.nowMotionData.motionMoveType is not none &&
                   ld.movePar.moveState is isGrounded;
        }

        public override string[] GetNodeFaceText()
        {
            var ikMotionData = MHUB.GetData(StaticInfo.Inst.nowEditMech.mechCustom.machineCode).machineCD.origHD.fightMover.fightMotionData[fightActionNum];
            var motionName = ikMotionData.Name;
            var tgtStr = targetList.GetIndicateStr();
            var enableMoveStr = ikMotionData.motionMoveType is not (none or jump) ? $"M:{(enableMove ? "O" : "X")} " : "";
            return new[] { $"{motionName}\nTGT:{tgtStr}\nH:{(enableHoming ? "O" : "X")} {enableMoveStr}T:{(enableThruster ? "O" : "X")}" };
        }
    }
}