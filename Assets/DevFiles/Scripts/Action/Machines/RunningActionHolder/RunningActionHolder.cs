using clrev01.Bases;
using clrev01.ClAction.Bullets;
using clrev01.ClAction.Machines.Motion;
using clrev01.ClAction.ObjectSearch;
using clrev01.Programs.FuncPar;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using System;
using System.Collections.Generic;
using clrev01.ClAction.Effect;
using clrev01.ClAction.Effect.MuzzleFlash;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;

namespace clrev01.ClAction.Machines.RunningActionHolder
{
    [System.Serializable]
    public abstract class RunningActionHolder<RAF> where RAF : ActionFuncPar
    {
        public virtual RAF RunningAction { get; protected set; }
        private RAF _nextAction;
        private bool _actionUpdateFlag;
        private bool _actionChangeFlag;
        public int actionStartFrame;

        public void ReplaceRunningAction(ActionFuncPar other, MachineLD ld)
        {
            var nowAction = !_actionUpdateFlag ? RunningAction : _nextAction;
            //干渉する動作の場合、
            if (nowAction != null && !nowAction.ActionOverwritable(other, ld)) return;
            _actionUpdateFlag = true;
            if (other is RAF raf)
            {
                //Holderに対応する動作なら次のActionを設定
                _nextAction = raf;
                _actionChangeFlag = _actionChangeFlag || RunningAction == null || RunningAction.GetType() != _nextAction.GetType() || _nextAction.CheckChangeAction(ld);
            }
            else
            {
                //Holderに対応しない動作なら次のActionをNullに
                _nextAction = null;
            }
        }
        public void RunExecute(MachineLD ld)
        {
            if (_actionUpdateFlag)
            {
                if (_nextAction != null)
                {
                    if (_actionChangeFlag)
                    {
                        RunningAction?.OnCancelRun(ld);
                        _nextAction.OnChangeAction(ld);
                        actionStartFrame = ACM.actionFrame;
                    }
                    _nextAction.InitOnExecute(ld);
                }
                else
                {
                    RunningAction?.OnCancelRun(ld);
                }
                RunningAction = _nextAction;
            }
            _actionUpdateFlag = false;
            _actionChangeFlag = false;
            _nextAction = null;

            if (RunningAction == null) return;
            if (!RunningAction.CheckIsExecutable(ld) || RunningAction.CheckEnd(ld))
            {
                EndAction(ld);
                return;
            }

            float energyCost = RunningAction.EnergyCost(ld);
            if (ld.statePar.energyUsed + energyCost <= ld.powerPlantData.energyCapacity)
            {
                ld.statePar.energyUsed += energyCost;
                if (RunningAction.ActionExecute(ld)) RunningAction = null;
            }
            else RunningAction = null;
        }

        public void EndAction(MachineLD ld)
        {
            RunningAction?.OnEndAction(ld);
            RunningAction = null;
        }

        public void CancelRun(MachineLD ld)
        {
            RunningAction?.OnCancelRun(ld);
            RunningAction = null;
        }
    }

    public class RunningMoveTypeHolder : RunningActionHolder<MoveTypeFuncPar>
    { }

    public class RunningRotateHolder : RunningActionHolder<RotateFuncPar>
    { }

    public class RunningShootHolder : RunningActionHolder<FireFuncPar>
    {
        public MachineLD ld;
        public override FireFuncPar RunningAction
        {
            get => base.RunningAction;
            protected set
            {
                if (value == null)
                {
                    numberOfContinuousShots = 0;
                }
                else
                {
                    if (numberOfContinuousShots == 0 || AimTgt != currentAimTgt || !aimingSustained)
                    {
                        nextShootAuto = value.waitMode is FireFuncPar.WaitMode.Auto;
                        startShoot = base.RunningAction is null;
                        startShootFrame = ACM.actionFrame;
                        currentAimTgt = AimTgt;
                    }
                    numberOfContinuousShots = value.numberOfContinuousShotsV.GetUseValueInt(ld, 0);
                }
                base.RunningAction = value;
            }
        }
        int _weapon;
        public int weapon
        {
            get => _weapon;
            set
            {
                _weapon = value;
                SetBullet(_weapon);
            }
        }
        public int numberOfShots = 0;
        public int numberOfContinuousShots = 0;
        public bool nextShootAuto;
        public bool startShoot;
        public int startShootFrame;
        public int currentShootFrame = 0;
        private float _nowWeaponRecoilWobbleRate = 0;
        private Vector3 _recoilVector = Vector3.zero;
        public Vector3 nowWeaponWobble = Vector3.zero;

        #region bullet

        [SerializeField]
        private IProjectileCommonData _bullet;
        public IProjectileCommonData bullet => _bullet;

        #endregion

        public List<ObjectSearchTgt> aimTgtList;
        public int aimTgtNumber;
        public bool isWeaponActive => RunningAction != null || aimTgtList is { Count: > 0 };
        public ObjectSearchTgt AimTgt
        {
            get
            {
                if (RunningAction is { targetingType: not TargetingType.AimLockOnTarget }) return null;
                if (aimTgtList is null || aimTgtNumber < 0 || aimTgtNumber >= aimTgtList.Count) return RunningAction?.fireTgtObj;
                var aimTgt = aimTgtList?[aimTgtNumber];
                if (RunningAction is { prioritizeAimTgt: FireFuncPar.PrioritizeTgtMode.PrioritizeFireTgt })
                {
                    return RunningAction.fireTgtObj != null ? RunningAction.fireTgtObj : aimTgt;
                }
                else
                {
                    return aimTgt != null ? aimTgt : RunningAction?.fireTgtObj;
                }
            }
        }
        public int latestAimInitFrame = int.MinValue;
        public int GetTgtChangeFrame =>
            RunningAction is { targetingType: not TargetingType.AimLockOnTarget } or { prioritizeAimTgt: FireFuncPar.PrioritizeTgtMode.PrioritizeFireTgt }
                ? RunningAction.latestFireInitFrame != int.MinValue ? RunningAction.latestFireInitFrame : latestAimInitFrame
                : latestAimInitFrame != int.MinValue
                    ? latestAimInitFrame
                    : RunningAction?.latestFireInitFrame ?? int.MinValue;
        private ObjectSearchTgt currentAimTgt { get; set; }
        public BaseAimIkPar AimIkPar { get; set; }
        public bool aimingCompleted =>
            RunningAction.targetingType switch
            {
                TargetingType.None => true,
                TargetingType.AimLockOnTarget => AimTgt == null || AimIkPar is null or { aimingCompleted: true },
                TargetingType.AimWithCoordinates => AimIkPar is null or { aimingCompleted: true },
                TargetingType.AimWithAngle => AimIkPar is null or { aimingCompleted: true },
                _ => throw new ArgumentOutOfRangeException()
            };
        public bool aimingSustained =>
            RunningAction.targetingType switch
            {
                TargetingType.None => true,
                TargetingType.AimLockOnTarget => AimTgt == null || AimIkPar is null or { aimingSustained: true },
                TargetingType.AimWithCoordinates => AimIkPar is null or { aimingSustained: true },
                TargetingType.AimWithAngle => AimIkPar is null or { aimingSustained: true },
                _ => throw new ArgumentOutOfRangeException()
            };

        private VfxSlaveObjectHD _muzzleFlashSlaveObj;
        private MuzzleFlashVfxControl _muzzleFlashVfxControl;

        public void SetBullet(int wc)
        {
            _bullet = WHUB.GetBulletCD(wc);
        }

        public void CalcWeaponWobble(float normalWobble)
        {
            nowWeaponWobble = _recoilVector * (normalWobble * _nowWeaponRecoilWobbleRate);
        }

        public void AddRecoil(float recoilDecayLate, float recoilWobbleRate)
        {
            _nowWeaponRecoilWobbleRate = (_nowWeaponRecoilWobbleRate + recoilWobbleRate) * recoilDecayLate;
            if (recoilWobbleRate > 0) _recoilVector = (_recoilVector + UnityEngine.Random.insideUnitSphere).normalized;
        }

        public void ExeMuzzleFlash(GameObject vfxMaster, Vector3 shootPosition, Vector3 shootDirection, Vector3 shooterSpeed)
        {
            if (!_muzzleFlashVfxControl)
            {
                _muzzleFlashSlaveObj = bullet.MuzzleFlashSpawn(shootPosition, shootDirection, shooterSpeed);
                if (!_muzzleFlashSlaveObj) return;
                _muzzleFlashSlaveObj.Init(vfxMaster);
                _muzzleFlashVfxControl = (MuzzleFlashVfxControl)_muzzleFlashSlaveObj.vfxController;
            }
            _muzzleFlashVfxControl?.PlayVfx(shootDirection * bullet.InitialSpeed + shooterSpeed, bullet.muzzleFlashSize);
        }

        public void UpdateMuzzleFlash()
        {
            _muzzleFlashVfxControl?.UpdateVfx();
        }
    }

    public class RunningFightHolder : RunningActionHolder<FightFuncPar>
    { }

    public class RunningDefenceHolder : RunningActionHolder<DefenceFuncPar>
    { }

    public class RunningThrustHolder : RunningActionHolder<ThrustFuncPar>
    { }

    public class RunningShieldHolder : RunningActionHolder<ShieldFuncPar>
    { }
}