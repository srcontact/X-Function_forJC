using System;
using clrev01.Bases;
using clrev01.ClAction.Machines.RunningActionHolder;
using clrev01.ClAction.ObjectSearch;
using clrev01.ClAction.Shield;
using clrev01.Extensions;
using clrev01.HUB;
using clrev01.PGE.VariableEditor;
using clrev01.Programs;
using clrev01.Programs.FuncPar;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using System.Collections.Generic;
using System.Linq;
using clrev01.ClAction.Machines.AdditionalTurret;
using clrev01.ClAction.Machines.Motion;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Machines
{
    [System.Serializable]
    public partial class MachineLD : LocalData<MachineCD, MachineLD, MachineHD>
    {
        public ActionState actionState;

        #region movePar

        [SerializeField]
        private MachineMovePar _movePar;
        public MachineMovePar movePar
        {
            get { return _movePar ??= new MachineMovePar(hd.moveCollider, ACM.actionFrame); }
        }

        #endregion

        public bool DuringLandingRigidity => movePar.landingRigidityEndFrame > ACM.actionFrame;

        #region customData

        [SerializeField, Tooltip("ユーザーがカスタマイズできるデータ。")]
        private CustomData _customData;
        public CustomData customData
        {
            get => _customData;
            set
            {
                _customData = value;
                //RegisterShootAction(customData);
                runningShootHolder = customData.mechCustom.weapons.Select((x, i) => new RunningShootHolder
                {
                    ld = this,
                    weapon = x,
                    AimIkPar = hd.aimList.Find(y => y.correspondingWeaponNum == i)
                }).ToList();
            }
        }

        #endregion

        #region pgExer

        [SerializeField]
        private PGExeData _pgExeData;
        public PGExeData pgExeData
        {
            get => _pgExeData ??= new PGExeData();
            set => _pgExeData = value;
        }

        #endregion

        public float weightSum = 0;
        public MachineStatePar statePar = new();
        public float HpPercent => HpRemaining / cd.maxHearthPoint;
        public float HpRemaining => Mathf.Max(cd.maxHearthPoint - statePar.damage, 0);
        public float HeatPercent => statePar.heat / cd.allowableTemperature;
        public bool outOfEnergy => statePar.energyUsed >= powerPlantData.energyCapacity;
        public ObjectSearchTgt latestAimTgt => runningShootHolder[GetLatestTgtChangeWeapon].AimTgt;
        public int GetLatestTgtChangeWeapon
        {
            get
            {
                var res = 0;
                var max = int.MinValue;
                for (var i = 0; i < runningShootHolder.Count; i++)
                {
                    var tgtChangeFrame = runningShootHolder[i].GetTgtChangeFrame;
                    if (tgtChangeFrame <= max) continue;
                    max = tgtChangeFrame;
                    res = i;
                }
                return res;
            }
        }
        public RunningMoveTypeHolder runningMoveTypeHolder = new();
        public RunningRotateHolder runningRotateHolder = new();
        ///↓初期化はcd.OnSetLd2で。
        public List<RunningShootHolder> runningShootHolder;
        public RunningFightHolder runningFightHolder = new();
        public RunningDefenceHolder runningDefenceHolder = new();
        public RunningThrustHolder runningThrustHolder = new();
        public RunningShieldHolder runningShieldHolder = new();
        public (int, StopProgramFuncPar)? stopCache;
        public PGData pGData => customData.program;
        public Queue<int> subroutineQueue = new();

        public ArmorTypeData armorTypeData => customData.mechCustom.ArmorTypeData;

        public ShieldCD shieldCd => customData.mechCustom.ShieldCd;
        public float ShieldHpRemaining => shieldCd.healthPoint - statePar.shieldDamage;
        public float ShieldHpPercent => ShieldHpRemaining / shieldCd.healthPoint;
        public float ImpactPercent => statePar.impact / cd.baseStability;
        public PowerPlantData powerPlantData => customData.mechCustom.PowerPlantData;
        public CpuData cpuData => customData.mechCustom.CpuData;
        public FcsData fcsData => customData.mechCustom.FcsData;
        public SearchParameterData searchParameterData => fcsData?.searchParameterData;
        public ThrusterData thrusterData => customData.mechCustom.ThrusterData;

        public bool AimingActive => movePar.moveState is not (CharMoveState.uncontrollable or CharMoveState.recovery) && runningFightHolder.RunningAction is null && runningDefenceHolder.RunningAction is null;

        public override void ResetEveryFrame()
        {
            base.ResetEveryFrame();
            movePar.accele = Vector3.zero;
            movePar.jumpV = Vector3.zero;
            movePar.tgtSpeed = Vector3.zero;
            movePar.thrustVector = Vector3.zero;
            movePar.nowBreakSleeping = false;
            if (runningRotateHolder.RunningAction == null) movePar.rotateTgtSpeed = Vector3.zero;
            hd.objectSearchTgt.jammingSize = armorTypeData.jammingSizeEffect;
            hd.objectSearchTgt.jammedSize = 0;
            weightSum = customData.mechCustom.CalcWeightSum(runningShootHolder, statePar.optionPartsUseCount);
            foreach (var ik in hd.ikList)
            {
                ik.GetIKSolver().IKPositionWeight = 0;
            }
        }

        public override void RunBeforePhysics()
        {
            if (statePar.destroyedFrame < 0 && statePar.damage >= cd.maxHearthPoint)
            {
                statePar.destroyedFrame = ACM.actionFrame;
                StaticInfo.Inst.battleExecutionData.AddNumberOfDefeat(hd.teamID);
            }
            if (statePar.destroyedFrame > 0)
            {
                if (statePar.destroyedFrame + cd.destroyMotionFrame < ACM.actionFrame)
                {
                    ExitOnDestroyed();
                }
                if (hd.shieldHd != null) hd.shieldHd.ShieldSetting(false, 0, Vector3.zero, false);
                movePar.ExecuteFallDown();
                actionState = ActionState.Uncontrollable;
                return;
            }

            if (actionState is ActionState.Dash or ActionState.Jump or ActionState.Breaking ||
                runningThrustHolder.RunningAction != null ||
                runningFightHolder.RunningAction != null)
            {
                statePar.energySupplyRestartFrame = ACM.actionFrame + powerPlantData.supplyRestartFrame;
            }
            var supplyRate = powerPlantData.energySupplyRate * (statePar.energySupplyRestartFrame < ACM.actionFrame ? 1 : 0);
            statePar.energyUsed = Mathf.Max(0, statePar.energyUsed - supplyRate);

            var radiationRate = Mathf.Pow(armorTypeData.radiationRate, customData.mechCustom.armorThickness);
            statePar.heat = Mathf.Max(StaticInfo.Inst.actionLevelHub.levels[StaticInfo.Inst.PlayMatch.playLevelNum].levelTemperature, statePar.heat - cd.baseCoolingPerformance * radiationRate);
            if (statePar.heat > cd.allowableTemperature)
            {
                statePar.damage += StaticInfo.Inst.machineCommonSetting.GetHeatDamage(statePar.heat - cd.allowableTemperature);
            }

            if (statePar.impact > 0)
            {
                switch (movePar.moveState)
                {
                    case CharMoveState.recovery:
                        statePar.impact = 0;
                        break;
                    case CharMoveState.inAir:
                        statePar.impact = Mathf.Max(statePar.impact - cd.stabilityRecoveryRateInAir, 0);
                        break;
                    case CharMoveState.isGrounded:
                        statePar.impact = Mathf.Max(statePar.impact - cd.stabilityRecoveryRate, 0);
                        break;
                }
            }

            if (shieldCd != null)
            {
                if (!statePar.shieldBreakFlag) statePar.shieldBreakFlag = statePar.shieldDamage >= shieldCd.healthPoint;
                if (statePar.latestShieldUseFrame + shieldCd.recoveryStartFrame < ACM.actionFrame)
                {
                    statePar.shieldDamage = Mathf.Min(Mathf.Max(statePar.shieldDamage - shieldCd.recoveryPointParFrame, 0), shieldCd.healthPoint);
                    if (statePar.shieldBreakFlag && statePar.shieldDamage < shieldCd.healthPoint * (1f - shieldCd.breakRecoveryRate)) statePar.shieldBreakFlag = false;
                }
            }

            if (variableValueDict.TryGetValue(VariableType.LockOn, out var ld))
            {
                foreach (var vl in ld.Values)
                {
                    var variableValueLockOn = (vl as VariableValueLockOn);
                    var lockOnTgt = variableValueLockOn?.GetLockOnValue();
                    if (lockOnTgt != null && !lockOnTgt.gameObject.activeSelf)
                    {
                        variableValueLockOn.SetLockOnValue(null);
                    }
                }
            }

            if (variableValueDict.TryGetValue(VariableType.LockOnList, out var lld))
            {
                foreach (var vll in lld.Values)
                {
                    var lockOnTgts = (vll as VariableValueLockOnList)?.Value;
                    if (lockOnTgts == null) continue;
                    for (var i = 0; i < lockOnTgts.Count; i++)
                    {
                        if (lockOnTgts[i] != null && !lockOnTgts[i].gameObject.activeSelf)
                        {
                            lockOnTgts[i] = null;
                        }
                    }
                }
            }

            movePar.StateCheck(this);

            base.RunBeforePhysics();

            if (!ACM.StartDelayNow) ProgramRun();

            SetActionState(movePar.moveState);

            movePar.Move(this);

            foreach (var shootHolder in runningShootHolder)
            {
                var aimTgt = shootHolder.AimTgt;
                if (aimTgt == null || aimTgt.aimingAtMeDict == null || aimTgt.aimingAtMeDict.ContainsKey(uniqueID)) continue;
                aimTgt.aimingAtMeDict.Add(uniqueID, hd.objectSearchTgt);
            }

            AimSettingExe();
        }

        private void SetActionState(CharMoveState charMoveState)
        {
            var currentActionState = actionState;
            switch (charMoveState)
            {
                case CharMoveState.inAir:
                    actionState = ActionState.InAir;
                    break;
                case CharMoveState.isGrounded:
                    if (statePar.impact > 0 && runningDefenceHolder.RunningAction == null)
                    {
                        actionState = ActionState.Breaking;
                    }
                    else if (runningMoveTypeHolder.RunningAction == null &&
                             runningRotateHolder.RunningAction == null &&
                             runningFightHolder.RunningAction == null &&
                             runningDefenceHolder.RunningAction == null)
                    {
                        actionState = ActionState.Neutral;
                    }
                    else if (runningMoveTypeHolder.RunningAction is BreakFuncPar)
                    {
                        actionState = ActionState.Breaking;
                    }
                    else if (runningMoveTypeHolder.RunningAction is DashFuncPar && currentActionState != ActionState.Dash)
                    {
                        actionState = ActionState.Dash;
                    }
                    else if (runningMoveTypeHolder.RunningAction is MoveFuncPar && currentActionState != ActionState.Move)
                    {
                        actionState = ActionState.Move;
                    }
                    else if (runningMoveTypeHolder.RunningAction is JumpFuncPar && currentActionState != ActionState.Jump)
                    {
                        actionState = ActionState.Jump;
                    }
                    else if (runningFightHolder.RunningAction != null)
                    {
                        switch (hd.fightMover.nowMotionData.motionMoveType)
                        {
                            case MotionMoveType.move:
                                actionState = ActionState.Move;
                                break;
                            case MotionMoveType.dash:
                                actionState = ActionState.Dash;
                                break;
                            case MotionMoveType.jump:
                                actionState = ActionState.Jump;
                                break;
                            case MotionMoveType.none:
                            default:
                                actionState = ActionState.Move;
                                break;
                        }
                    }
                    else if (runningDefenceHolder.RunningAction != null)
                    {
                        actionState = runningDefenceHolder.RunningAction.actionNum switch
                        {
                            DefenceActionType.Guard => ActionState.Guard,
                            DefenceActionType.Cover => ActionState.Cover,
                            _ => throw new ArgumentOutOfRangeException()
                        };
                    }
                    else if (runningRotateHolder.RunningAction != null && currentActionState != ActionState.Move)
                    {
                        actionState = ActionState.Move;
                    }
                    else
                    {
                        actionState = currentActionState;
                    }

                    break;
                case CharMoveState.uncontrollable:
                    actionState = ActionState.Uncontrollable;
                    break;
                case CharMoveState.recovery:
                    actionState = ActionState.Recovery;
                    break;
                default:
                    actionState = ActionState.Neutral;
                    break;
            }
        }

        public void AimSettingExe()
        {
            if (!AimingActive) return;
            foreach (var aimPar in hd.aimList)
            {
                if (aimPar is AdditionalTurretAimIkPar atAimPar)
                {
                    if (!atAimPar.additionalTurretObj) continue;
                    if (!runningShootHolder[aimPar.correspondingWeaponNum].isWeaponActive)
                    {
                        atAimPar.additionalTurretObj.animationController.UpdateAnimation((int)AdditionalTurretState.DefaultPose);
                        continue;
                    }
                    atAimPar.additionalTurretObj.animationController.UpdateAnimation((int)AdditionalTurretState.AimingPose);
                }
                var iKSolver = aimPar.useAimIk.GetIKSolver();
                iKSolver.IKPositionWeight = 1;
            }
        }

        public void ShootExe()
        {
            var shooterSpeed = movePar.rBody.linearVelocity;
            for (var i = 0; i < runningShootHolder.Count; i++)
            {
                var rsh = runningShootHolder[i];
                if (rsh.bullet == null) continue;
                var tgt = rsh.AimTgt != null ? rsh.AimTgt : null;

                if (rsh.RunningAction != null &&
                    rsh.numberOfShots < customData.mechCustom.weaponAmoNum[i] &&
                    rsh.currentShootFrame + Mathf.Max(rsh.RunningAction.intervalV.GetUseValueInt(this, 0), rsh.bullet.MinimumFiringInterval) < ACM.actionFrame &&
                    rsh.numberOfContinuousShots > 0 &&
                    (
                        !rsh.startShoot ||
                        (!rsh.nextShootAuto && rsh.startShootFrame + rsh.RunningAction.waitBeforeShootV.GetUseValueInt(this, 0) < ACM.actionFrame) ||
                        (rsh.nextShootAuto && (rsh.aimingCompleted || rsh.startShootFrame + rsh.RunningAction.waitBeforeShootV.GetUseValueInt(this, 0) < ACM.actionFrame))
                    )
                   )
                {
                    rsh.nextShootAuto = false;
                    rsh.startShoot = false;
                    var shootPosition = hd.useShootPoints[i].position;
                    var shootDirection = AimingActive && rsh.AimIkPar is { aimingCompleted: true } ? (rsh.AimIkPar.ikPos - shootPosition).normalized : hd.useShootPoints[i].forward;
                    rsh.bullet.Shoot(shootPosition, shootDirection, shooterSpeed, tgt, hd.objectSearchTgt, hd.teamID, hd.uniqueID);
                    rsh.ExeMuzzleFlash(hd.useShootPoints[i].gameObject, shootPosition, shootDirection, shooterSpeed);
                    rsh.numberOfShots++;
                    rsh.numberOfContinuousShots--;
                    rsh.currentShootFrame = ACM.actionFrame;
                    rsh.AddRecoil(cd.usableWeapons[i].recoilWobbleDecayLate, rsh.bullet.RecoilPower);
                    var fireImpact = movePar.moveState is CharMoveState.isGrounded ? Mathf.Max(rsh.bullet.RecoilPower - cd.baseImpactResistValue, 0) : rsh.bullet.RecoilPower;
                    statePar.impact += fireImpact;
                    if (statePar.impact < cd.baseStability) hd.rigidBody.AddForce(-shootDirection * fireImpact / cd.baseImpactPhysicsRate, ForceMode.VelocityChange);
                    else hd.rigidBody.AddForceAtPosition(-shootDirection * fireImpact / cd.baseImpactPhysicsRate, shootPosition, ForceMode.VelocityChange);
                    statePar.heat += rsh.bullet.HeatValueAtFire;
                    statePar.energyUsed += rsh.bullet.UseEnergyOnFire;
                    if (rsh.bullet.UseEnergyOnFire > 0)
                    {
                        statePar.energySupplyRestartFrame = ACM.actionFrame + powerPlantData.supplyRestartFrame;
                    }
                }
                else
                {
                    rsh.AddRecoil(cd.usableWeapons[i].recoilWobbleDecayLate, 0);
                }
                rsh.UpdateMuzzleFlash();

                rsh.CalcWeaponWobble(cd.usableWeapons[i].normalWobbleLate);
            }
        }

        public virtual void DebugAcceleAdd()
        {
            Vector3 mv = Vector3.zero;
            float vi = Input.GetAxisRaw("Vertical");
            if (System.Math.Abs(vi) > Mathf.Epsilon)
            {
                if (vi.isBiggerV3(0)) mv += Vector3.forward;
                else mv += Vector3.back;
            }

            float hi = Input.GetAxisRaw("Horizontal");
            if (System.Math.Abs(hi) > Mathf.Epsilon)
            {
                if (hi.isBiggerV3(0)) mv += Vector3.right;
                else mv += Vector3.left;
            }

            if (mv.isZeroV3()) return;
            movePar.ActMove(mv, (vi + hi) / 2);
        }

        /// <summary>
        /// ノードの処理を次のフレームに持ち越すフラグ
        /// </summary>
        private bool _nodeCarryOverNextFrame;

        public void ProgramRun()
        {
            if (pgExeData.pgbdDict.Count <= 0) return;
            CalcCacheClear();
            var useEn = cpuData.useEnergyPerFrame;
            statePar.energyUsed += useEn;
            if (outOfEnergy)
            {
                statePar.energyUsed = powerPlantData.energyCapacity;
                runningMoveTypeHolder.CancelRun(this);
                runningRotateHolder.CancelRun(this);
                runningShootHolder.ForEach(x => x.CancelRun(this));
                runningThrustHolder.CancelRun(this);
                runningShieldHolder.CancelRun(this);
                runningFightHolder.CancelRun(this);
                runningDefenceHolder.CancelRun(this);
                return;
            }

            pgExeData.exeResource += cpuData.exeResourceSupply;
            if (pgExeData.exeResource > cpuData.exeResourceSupply) pgExeData.exeResource = cpuData.exeResourceSupply;

            for (int i = 0; i < 10000; i++)
            {
                PGBData nowExecute = pgExeData.pgbdDict[pgExeData.pgExePos];
                if (!_nodeCarryOverNextFrame) pgExeData.exeResource -= nowExecute.funcPar.calcCost;
                if (pgExeData.exeResource <= 0)
                {
                    _nodeCarryOverNextFrame = true;
                    break;
                }
                _nodeCarryOverNextFrame = false;

                PGBExe(nowExecute, out var branchFlag, out var endFlag, out var goSubroutine);
                int next;
                if (goSubroutine.HasValue)
                {
                    if (nowExecute.GetReturnFlag())
                    {
                        if (subroutineQueue.Count == 0) subroutineQueue.Enqueue(0);
                    }
                    else
                    {
                        subroutineQueue.Enqueue(nowExecute.editorPar.nextIndex);
                    }
                    next = goSubroutine.Value;
                }
                else
                {
                    next = branchFlag ? nowExecute.editorPar.nextIndex : nowExecute.editorPar.falseNextIndex;
                }
                if (endFlag) break;
                if (next == pgExeData.pgExePos || next < 0)
                {
                    if (subroutineQueue.Count > 0)
                    {
                        next = subroutineQueue.Dequeue();
                    }
                    else
                    {
                        next = 0;
                    }
                }
                pgExeData.pgExePos = next;
            }
            runningMoveTypeHolder.RunExecute(this);
            runningRotateHolder.RunExecute(this);
            runningFightHolder.RunExecute(this);
            runningDefenceHolder.RunExecute(this);
            runningThrustHolder.RunExecute(this);
            runningShieldHolder.RunExecute(this);
            foreach (var raf in runningShootHolder)
            {
                raf.RunExecute(this);
            }
            for (var i = 0; i < customData.mechCustom.optionParts.Count; i++)
            {
                if (!statePar.optionPartsUseFrameDict.ContainsKey(i) || statePar.optionPartsUseFrameDict[i] < ACM.actionFrame) continue;
                var op = OpHub.GetOptionPartsData(customData.mechCustom.optionParts[i]);
                op.data.ExeOptionParts(this, i);
            }
        }

        private void CalcCacheClear()
        { }

        private void PGBExe(PGBData pgbd, out bool branchFlag, out bool endFlag, out int? goSubroutine)
        {
            pgbd.currentExecutedFrame = ACM.actionFrame;
            branchFlag = true;
            endFlag = false;
            goSubroutine = null;
            int myIndex = pgbd.editorPar.myIndex;
            switch (pgbd.funcPar)
            {
                case FireFuncPar x:
                    if (!x.CheckIsExecutable(this)) break;
                    runningShootHolder[x.fireWeapon].ReplaceRunningAction(x, this);
                    break;
                case ActionFuncPar x:
                    if (!x.CheckIsExecutable(this)) break;
                    runningMoveTypeHolder.ReplaceRunningAction(x, this);
                    runningRotateHolder.ReplaceRunningAction(x, this);
                    runningFightHolder.ReplaceRunningAction(x, this);
                    runningDefenceHolder.ReplaceRunningAction(x, this);
                    runningThrustHolder.ReplaceRunningAction(x, this);
                    runningShieldHolder.ReplaceRunningAction(x, this);
                    foreach (var raf in runningShootHolder)
                    {
                        raf.ReplaceRunningAction(x, this);
                    }
                    break;
                case BranchFuncPar x:
                    branchFlag = x.BranchExecute(this);
                    break;
                case StartFuncPar x:
                    break;
                case SubroutineRootFuncPar x:
                    break;
                case LockOnFuncPar x:
                    x.LockOn(this);
                    break;
                case AimFuncPar x:
                    Aim(x, myIndex);
                    break;
                case UseOptionalFuncPar x:
                    x.ExeUseOptional(this);
                    break;
                case StopProgramFuncPar x:
                    endFlag = StopProgram(x, myIndex);
                    break;
                case CalcFuncPar x:
                    x.ExeCalcNumeric(this);
                    break;
                case CalcVector3dFuncPar x:
                    x.ExeCalcVector3d(this);
                    break;
                case CalcNumericListFuncPar x:
                    x.ExeCalcNumericList(this);
                    break;
                case CalcVector3dListFuncPar x:
                    x.ExeCalcVector3dList(this);
                    break;
                case ManageLockOnFuncPar x:
                    x.ExeManageLockOn(this);
                    break;
                case LockOnListRemoveNullFuncPar x:
                    x.ExeLockOnListRemoveNull(this);
                    break;
                case IGetStatusValueFuncPar x:
                    x.GetStatusValue(this);
                    break;
                case SubroutineExecuteFuncPar x:
                    goSubroutine = GoToSubroutine(x, myIndex);
                    break;
            }
        }

        private void Aim(AimFuncPar funcPar, int myNum)
        {
            ObjectSearchTgt aimTgt = funcPar.aimTgtList.GetUseValue(this);
            if (aimTgt == null)
            {
                runningShootHolder.ForEach(x => x.aimTgtList = null);
                return;
            }
            var lockOnList = funcPar.aimTgtList.GetUseList(this);
            for (int i = 0; i < runningShootHolder.Count; i++)
            {
                if ((funcPar.aimWeaponFlags & 1L << i) == 0) continue;
                runningShootHolder[i].aimTgtList = lockOnList;
                //todo:Aimターゲットとして単体型LockOnを指定するとロックオンできない状態のはず。直すべし。aimTgtListはObjectSearchTarget型単体に直していいはず。
                runningShootHolder[i].aimTgtNumber = funcPar.aimTgtList.indexV.GetUseValue(this);
                runningShootHolder[i].latestAimInitFrame = ACM.actionFrame;
            }
        }

        private bool StopProgram(StopProgramFuncPar x, int mn)
        {
            stopCache ??= (ACM.actionFrame, x);

            if (stopCache.Value.Item1 + stopCache.Value.Item2.durationV.GetUseValueInt(this, 1) > ACM.actionFrame)
            {
                return true;
            }
            else
            {
                stopCache = null;
                return false;
            }
        }

        public bool AssessAction(AssessActionType actionState, long weaponNum)
        {
            switch (actionState)
            {
                case AssessActionType.Neutral:
                    return runningMoveTypeHolder.RunningAction == null;
                case AssessActionType.Move:
                    return runningMoveTypeHolder.RunningAction is MoveFuncPar;
                case AssessActionType.Dash:
                    return runningMoveTypeHolder.RunningAction is DashFuncPar;
                case AssessActionType.Jump:
                    return runningMoveTypeHolder.RunningAction is JumpFuncPar;
                case AssessActionType.Brake:
                    return runningMoveTypeHolder.RunningAction is BreakFuncPar;
                case AssessActionType.Rotate:
                    return runningRotateHolder.RunningAction != null;
                case AssessActionType.Fire:
                    for (int i = 0; i < runningShootHolder.Count; i++)
                    {
                        var l = 1L << i;
                        if ((weaponNum & l) != l) continue;
                        if (runningShootHolder[i].RunningAction != null) return true;
                    }
                    return false;
                case AssessActionType.Thrust:
                    return runningThrustHolder.RunningAction != null;
                case AssessActionType.Fight:
                    return runningFightHolder.RunningAction != null;
                case AssessActionType.Defense:
                    return runningDefenceHolder.RunningAction != null;
                case AssessActionType.UseOptionalParts:
                    foreach (var op in statePar.optionPartsUseFrameDict)
                    {
                        var l = 1L << op.Key;
                        if ((weaponNum & l) != l) continue;
                        if (op.Value >= ACM.actionFrame) return true;
                    }
                    return false;
                case AssessActionType.Grounded:
                    return movePar.moveState is CharMoveState.isGrounded;
                case AssessActionType.InAir:
                    return movePar.moveState is CharMoveState.inAir;
                case AssessActionType.ImpactAbsorbing:
                    return movePar.moveState is CharMoveState.isGrounded && statePar.impact > 0;
                case AssessActionType.Controllable:
                    return movePar.moveState is not CharMoveState.uncontrollable and not CharMoveState.recovery;
                case AssessActionType.Uncontrollable:
                    return movePar.moveState is CharMoveState.uncontrollable;
                case AssessActionType.Recovery:
                    return movePar.moveState is CharMoveState.recovery;
                case AssessActionType.Shield:
                    return runningShieldHolder.RunningAction != null;
                case AssessActionType.Aim:
                default:
                    return false;
            }
        }

        private int GoToSubroutine(SubroutineExecuteFuncPar x, int mn)
        {
            return x.subroutineRootToGo;
        }

        public float CalcWeightPar()
        {
            return (weightSum - cd.mechWeight) / cd.maxAdditionalLoadingCap;
        }

        public override void RegisterTag(string nTag)
        {
            base.RegisterTag(nTag);
            foreach (var c in hd.colliderList)
            {
                c.tag = nTag;
            }
        }

        private void ExitOnDestroyed()
        {
            hd.gameObject.SetActive(false);
            hd.ExplosionExe();
        }
    }
}