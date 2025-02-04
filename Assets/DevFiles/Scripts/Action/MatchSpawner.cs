using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.Extensions;
using clrev01.HUB;
using clrev01.Save;
using MapMagic.Core;
using System;
using clrev01.ClAction.Bullets;
using UnityEngine;
using static clrev01.Bases.UtlOfCL;
using static clrev01.HUB.ActionLevelHub;
using Random = UnityEngine.Random;

namespace clrev01.ClAction
{
    public class MatchSpawner : BaseOfCL
    {
        public bool doSpawn = false;
        public bool spawned;
        public MatchData matchData => StaticInfo.Inst.PlayMatch;
        public LevelHolder LevelMesh => StaticInfo.Inst.actionLevelHub.levels[StaticInfo.Inst.PlayMatch.playLevelNum];
        public ActionLevelSize LevelSize => StaticInfo.Inst.actionLevelHub.levelSizes[matchData.levelSizeNumber];

        public int LevelSizeMargin => StaticInfo.Inst.actionLevelHub.levelSizeMargin;
        public int RadarCameraMargin => StaticInfo.Inst.actionLevelHub.radarCameraMargin;

        public Transform groundParent;
        public Transform wallObject;
        public MapMagicObject mapMagicObject;
        public UnityEngine.Camera actionRadarCamera;
        public float distFromCenter => LevelSize.size * 0.85f / 2;
        public float teamWidth => LevelSize.size * 0.85f / 2;


        void OnEnable()
        {
            if (!doSpawn) return;
            ACM.ResetAction();

            wallObject.localScale = new Vector3(LevelSize.size, 1, LevelSize.size);
            actionRadarCamera.orthographicSize = (LevelSize.size + RadarCameraMargin) / 2f;
        }

        private void Start()
        {
            MapMagicInit();
        }

        private void MapMagicInit()
        {
            if (!doSpawn) return;
            var graph = LevelMesh.mapMagicGraph.SafeInstantiate();
            graph.random.Seed = Random.Range(int.MinValue, int.MaxValue);
            mapMagicObject.globals.height = LevelMesh.height;
            var size = LevelSize.size + LevelSizeMargin * 2;
            mapMagicObject.tileSize = new Den.Tools.Vector2D(size, size);
            mapMagicObject.transform.position = new Vector3(-(float)size / 2, 0, -(float)size / 2);
            var resolution =
                ((MapMagicObject.Resolution[])Enum.GetValues(typeof(MapMagicObject.Resolution)))[Mathf.Min(LevelMesh.mapResolution + matchData.levelSizeNumber, 6)];
            mapMagicObject.tileResolution = resolution;
            mapMagicObject.Refresh(true);
            mapMagicObject.graph = graph;
            mapMagicObject.ApplyTileSettings();
            mapMagicObject.enabled = false;
            mapMagicObject.enabled = true;
        }

        public bool MapSpawnCheck()
        {
            return Physics.Raycast(Vector3.up * 1000, Vector3.down * 10000);
        }

        public MachineHD SpawnMachine(CustomData data, int teamNum, int machineIdInTeam, Vector3 spawnPos, float direction)
        {
            Vector3 spos = spawnPos + new Vector3(Random.value, 1000, Random.value);
            RaycastHit rh;
            Physics.Raycast(spos, Vector3.down * 10000, out rh);
            spos = rh.point + Vector3.up * 3;
            Quaternion srot = Quaternion.Euler(0, direction + Random.value, 0);
            MachineHD mech = data.InstActor(spos, srot);
            mech.ld.RegisterTag("team" + (teamNum + 1).ToString());
            mech.teamID = teamNum;
            mech.machineIdInTeam = machineIdInTeam;
            var mcs = MHUB.GetData(data.mechCustom.machineCode);
            mech.gameObject.name = $"T{teamNum}-{machineIdInTeam}_{mcs.MachineName}";
            mech.SetRadarSymbolMat(teamNum);
            ACM.machineList.Add(mech);

            for (int i = 0; i < data.mechCustom.weapons.Count; i++)
            {
                int wCode = data.mechCustom.weapons[i];
                var bulletCd = WHUB.GetData(wCode).bulletCD;
                if (bulletCd is null) continue;
                var weaponAmoNum = data.mechCustom.weaponAmoNum[i];
                StandbyBullet(weaponAmoNum, bulletCd);
                if (bulletCd is AirTurretDroneCD airTurretDroneCD)
                {
                    StandbyBullet(weaponAmoNum, airTurretDroneCD.origBullet);
                }
            }
            for (var i = 0; i < data.mechCustom.optionParts.Count; i++)
            {
                if (i < 0 || i >= data.mechCustom.optionParts.Count || i >= data.mechCustom.optionPartsUsableNum.Count) continue;
                var code = data.mechCustom.optionParts[i];
                var num = data.mechCustom.optionPartsUsableNum[i];
                OpHub.GetOptionPartsData(code)?.data?.StandbyPoolActors(num);
            }

            return mech;
        }
        private static void StandbyBullet(int weaponAmoNum, IProjectileCommonData bulletCd)
        {
            int ammoNum = weaponAmoNum * bulletCd.SimultaneousFiringNum;
            float maxSpawnNum = (float)bulletCd.LifeFrame / (bulletCd.MinimumFiringInterval + 1) * bulletCd.SimultaneousFiringNum;
            float standbyNum = Mathf.Min(ammoNum, maxSpawnNum);
            // Debug.Log($"Bullet初期スポーン\n{bulletCd}_{standbyNum}_{ammoNum}_{maxSpawnNum}");
            bulletCd.StandbyPoolActors(Mathf.CeilToInt(standbyNum));
        }
        public void SpawnTeam(TeamData data, int teamNum, Vector3 center, float teamDirection)
        {
            var machines = data.machineList.FindAll(x => x != null);
            for (int i = 0; i < machines.Count; i++)
            {
                if (machines[i] == null) continue;
                Vector3 sp = center + (machines.Count == 1 ? Vector3.zero : new Vector3(-teamWidth / 2 + teamWidth / (machines.Count - 1) * i, 0, 0));
                SpawnMachine(machines[i], teamNum, i, sp, teamDirection);
            }
        }
        public void SpawnMatchTwoTeam()
        {
            if (!doSpawn) return;
            SpawnTeam(matchData.teamList[0], 0, new Vector3(0, 0, distFromCenter), 180);
            SpawnTeam(matchData.teamList[1], 1, new Vector3(0, 0, -distFromCenter), 0);
            Physics.SyncTransforms();
            spawned = true;
        }
    }
}