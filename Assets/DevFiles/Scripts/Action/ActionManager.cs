using clrev01.Bases;
using clrev01.ClAction.Bullets;
using clrev01.ClAction.Machines;
using clrev01.ClAction.UI;
using clrev01.PGE.PGBView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static clrev01.ClAction.UI.ExFpsCounter;

namespace clrev01.ClAction
{
    public class ActionManager : SingletonOfCL<ActionManager>
    {
        public List<HardBase> RunList = new();
        public List<HardBase> AddList = new();

        public ActionEnvironmentPar actionEnvPar;
        public ActionIndicater actionIndicator;
        public PGBView programView;
        public bool searchFieldIndicate;
        public int exeFrame = 0, actionFrame = -60;
        public bool StartDelayNow => actionFrame < 0;

        #region fpsCount

        #region fpsCounter

        [Header("FPSカウント関連")]
        public Fps fps;
        [SerializeField]
        private FpsCounter _fpsCounter;
        public FpsCounter fpsCounter => _fpsCounter;

        #endregion

        #region fixedFpsCounter

        [SerializeField]
        private FpsCounter _fixedFpsCounter;
        public FpsCounter fixedFpsCounter => _fixedFpsCounter;

        #endregion

        #endregion

        public Dictionary<int, IHaveHitCollider> colliderDict = new();
        public List<MachineHD> machineList = new();
        [SerializeField]
        private int _cameraTgtMachine = 0;
        public int cameraTgtMachine
        {
            get => _cameraTgtMachine;
            set
            {
                _cameraTgtMachine = value;
                actionIndicator.ChangeMachineInfoIndicate();
            }
        }

        [SerializeField]
        private bool _doParentSet = true;
        public bool doParentSet
        {
            get
            {
#if UNITY_EDITOR
                return _doParentSet;
#endif
                return false;
            }
        }

        public FrameParSec frameParSec
        {
            get => StaticInfo.Inst.ActionSettingData.frameParSec;
            set => StaticInfo.Inst.ActionSettingData.frameParSec = value;
        }

        bool _pauseOnOff;
        public bool pauseOnOff
        {
            get => _pauseOnOff || actionMenu.gameObject.activeSelf;
            private set => _pauseOnOff = value;
        }
        public ActionMenu actionMenu;
        [SerializeField]
        private MatchSpawner _matchSpawner;
        [SerializeField]
        private BattleManager _battleManager;
        public int GetFrameRemaining => _battleManager.actionEndFrame - actionFrame;

        public Bounds areaLimitBounds { get; private set; }
        private IEnumerator _afterFixedUpdateCoroutine;


        private void OnEnable()
        {
            if (!StaticInfo.Inst.fixedRandomSeed)
            {
                StaticInfo.Inst.SetUnityRandomSeedUseDateTimeNow();
            }
            Random.InitState(StaticInfo.Inst.unityRandomSeed);
            HardBase.ResetUniqueId();
            LocalDataBase.ResetUniqueId();
            IProjectileCommonData.ResetFiringId();
            Resources.UnloadUnusedAssets();
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            actionMenu.Initialize();
            areaLimitBounds = new Bounds(Vector3.zero, Vector3.one * StaticInfo.Inst.actionLevelHub.levelSizes[StaticInfo.Inst.PlayMatch.levelSizeNumber].size);
            _afterFixedUpdateCoroutine = CoroutineRunAfterFixedUpdate();
            StartCoroutine(_afterFixedUpdateCoroutine);
        }

        private void OnDisable()
        {
            StopCoroutine(_afterFixedUpdateCoroutine);
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            Time.timeScale = 1;
            Time.fixedDeltaTime = 1f / 60;
        }

        private void Start()
        {
            Physics.gravity = new Vector3(0, -actionEnvPar.globalGPowMSec, 0);
            //Debug.Log((layerOfBullet + layerOfMachine) + "**" + (layerOfMachine + layerOfBullet));
        }

        private void Update()
        {
            if (!pauseOnOff && !_battleManager.endAction)
            {
                Time.timeScale = 1;
                if (actionFrame < -1)
                {
                    Time.fixedDeltaTime = 1f / 60;
                }
                else
                {
                    Time.fixedDeltaTime = 1f / (int)frameParSec;
                }
            }
            else
            {
                Time.timeScale = 0;
            }

            if (_matchSpawner.spawned)
            {
                if (actionIndicator != null) actionIndicator.RunOnUpdate();
                if (programView != null) programView.RunOnUpdate();
            }

            for (var i = RunList.Count - 1; i >= 0; i--)
            {
                RunList[i].RunOnUpdate();
            }

            fpsCounter.Run();
            fps.RunOnUpdate();
        }

        private void FixedUpdate()
        {
            if (!_matchSpawner.spawned)
            {
                if (!_matchSpawner.MapSpawnCheck()) return;
                HardBase.ResetUniqueId();
                LocalDataBase.ResetUniqueId();
                IProjectileCommonData.ResetFiringId();
                _matchSpawner.SpawnMatchTwoTeam();
                StaticInfo.Inst.battleExecutionData.InitResultData(StaticInfo.Inst.PlayMatch.teamList.Count);
                actionIndicator.ChangeMachineInfoIndicate();
            }

            ActionUpdateOnFixedUpdate();

            fixedFpsCounter.Run();
            exeFrame++;
        }

        private void ActionUpdateOnFixedUpdate()
        {
            actionFrame++;
            AddNewHards();
            _battleManager.CheckInitialize();
            foreach (var runner in RunList)
            {
                if (runner.objectSearchTgt == null || !runner.gameObject.activeSelf) continue;
                runner.objectSearchTgt.RegisterSearchTgt();
            }

            foreach (var runner in RunList)
            {
                if (!runner.gameObject.activeSelf) continue;
                runner.ResetEveryFrame();
            }

            foreach (var runner in RunList)
            {
                if (!runner.gameObject.activeSelf) continue;
                runner.RunInitialFrame();
            }

            foreach (var runner in RunList)
            {
                if (!runner.gameObject.activeSelf) continue;
                runner.RunBeforePhysics();
            }

            Physics.SyncTransforms();
            Physics.Simulate(1f / 60);

            foreach (var runner in RunList)
            {
                if (runner.objectSearchTgt != null && runner.objectSearchTgt.aimingAtMeDict != null)
                    runner.objectSearchTgt.aimingAtMeDict.Clear();
            }

            foreach (var runner in RunList)
            {
                if (!runner.gameObject.activeSelf) continue;
                runner.RunAfterPhysics();
                if (runner is IIkUser ikUser) ikUser.FixTransformsIk();
            }
        }

        private void ActionUpdateAfterPhysicsAndAnimation()
        {
            foreach (var runner in RunList)
            {
                runner.RunOnAfterFixedUpdateAndAnimation();
            }

            for (var i = RunList.Count - 1; i >= 0; i--)
            {
                switch (RunList[i])
                {
                    case null:
                        break;
                    case MachineHD x:
                        _battleManager.CheckMachineState(x);
                        break;
                    case BulletHD x:
                        _battleManager.CheckBulletState(x);
                        break;
                }

                var notActive = !RunList[i].gameObject.activeSelf;
                var ignoreRegisterFlag =
                    RunList[i].objectSearchTgt != null && RunList[i].objectSearchTgt.ignoreRegisterFlag;
                if (notActive || ignoreRegisterFlag)
                {
                    if (RunList[i].objectSearchTgt != null) RunList[i].objectSearchTgt.UnregisterSearchTgt();
                }

                if (notActive)
                {
                    RemoveRun(i);
                }
            }

            _battleManager.AggregateEndAction();
        }

        private IEnumerator CoroutineRunAfterFixedUpdate()
        {
            var waitForFixedUpdate = new WaitForFixedUpdate();
            while (true)
            {
                yield return waitForFixedUpdate;
                if (!_matchSpawner.spawned && !_matchSpawner.MapSpawnCheck()) continue;
                foreach (var runner in RunList)
                {
                    if (runner is IIkUser ikUser)
                    {
                        ikUser.UpdateIk();
                    }
                }
                ActionUpdateAfterPhysicsAndAnimation();
            }
        }

        private void AddNewHards()
        {
            if (AddList.Count > 0)
            {
                RunList.AddRange(AddList);
                AddList.Clear();
            }
        }

        public void AddRun(HardBase ahd)
        {
            //if (!ahd.LdIsAlive()) return;
            AddList.Add(ahd);
        }

        private void RemoveRun(int index)
        {
            var ahd = RunList[index];
            RunList.RemoveAt(index);
            ahd.objectPool.PoolingObject(ahd);
        }

        public void ResetAction()
        {
            for (int i = RunList.Count - 1; i >= 0; i--)
            {
                RemoveRun(i);
            }
        }

        public IHaveHitCollider GetHardFromCollider(Collider c)
        {
            if (c == null || !colliderDict.ContainsKey(c.GetInstanceID())) return null;
            return colliderDict[c.GetInstanceID()];
        }

        public void PauseChange()
        {
            pauseOnOff = !pauseOnOff;
        }
    }
}