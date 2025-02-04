using clrev01.ClAction.ActionCamera;
using clrev01.ClAction.Machines;
using clrev01.HUB;
using clrev01.Menu;
using clrev01.PGE.Undo;
using clrev01.Save;
using MemoryPack;
using MemoryPack.Compression;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using clrev01.ClAction;
using clrev01.Menu.BattleMenu;
using clrev01.Menu.BattleMenu.Arena;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Cysharp.Text;

namespace clrev01.Bases
{
    public class StaticInfo : SingletonOfCL<StaticInfo>
    {
        public bool fixedRandomSeed
        {
            get => menuSaveData?.testMatch?.fixedRandomSeed ?? false;
            set => menuSaveData.testMatch.fixedRandomSeed = value;
        }
        public int unityRandomSeed
        {
            get => menuSaveData?.testMatch?.randomSeed ?? 0;
            set => menuSaveData.testMatch.randomSeed = value;
        }
        public int actionEndFrame
        {
            get => menuSaveData?.testMatch?.actionEndFrame ?? 0;
            set => menuSaveData.testMatch.actionEndFrame = value;
        }
        public ColorBlockAsset normalColorInfo;
        public ColorBlockAsset toggledColorInfo;
        public Sprite defaultIcon;
        public CustomData nowEditMech
        {
            get => menuSaveData.nowEditMech;
            set => menuSaveData.nowEditMech = value;
        }
        public TeamData nowEditTeam
        {
            get => menuSaveData.nowEditTeam;
            set => menuSaveData.nowEditTeam = value;
        }

        #region playMatch

        public MatchData PlayMatch { get; set; }

        #endregion

        [SerializeField]
        List<int> sceneLog = new List<int>();
        public int nowPage { get; set; } = -1;
        [ReadOnly]
        public List<int> pageLog = new List<int>();
        public MatchData testMatch
        {
            get => menuSaveData.testMatch;
            set => menuSaveData.testMatch = value;
        }

        public int selectedArena;
        public int selectedArenaBattle;
        public TeamData arenaMyTeam
        {
            get => menuSaveData.arenaMyTeam;
            set => menuSaveData.arenaMyTeam = value;
        }

        public MachineHub machineHub;
        public WeaponHub weaponHub;
        public RadarSymbolHub radarSymbolHub;
        public ActionLevelHub actionLevelHub;
        private MenuSaveData menuSaveData { get; set; } = new MenuSaveData();
        public ArenaSaveData arenaSaveData { get; set; } = new ArenaSaveData();
        public BattleExecutionData battleExecutionData { get; set; } = new BattleExecutionData();
        private string menuSaveFileName => Path.Combine(savePath, "MenuData.xfd");
        private string arenaSaveFileName => Path.Combine(savePath, "ArenaSaveData.xfd");
        public ActionSettingData ActionSettingData { get; set; } = new ActionSettingData();
        private string actionSettingFileName => Path.Combine(savePath, "ActionSetting.xfd");
        private string savePath
        {
            get
            {
#if UNITY_EDITOR
                return Path.Combine(Application.dataPath, "SaveData");
#else
            return Path.Combine(Application.persistentDataPath, "SaveData");
#endif
            }
        }
        public UndoManager UndoManager { get; set; } = new UndoManager();

        [System.Serializable]
        public class MachineCommonSetting
        {
            [SerializeField]
            float heatDamageGraphHorizontalRatio = 2000;
            [SerializeField]
            int heatDamageGraphVerticalRatio = 1;
            [SerializeField]
            AnimationCurve heatDamageGraph;

            public int GetHeatDamage(float overAllowableTemperature)
            {
                return Mathf.CeilToInt(heatDamageGraph.Evaluate(overAllowableTemperature / heatDamageGraphHorizontalRatio) * heatDamageGraphVerticalRatio);
            }
        }

        public MachineCommonSetting machineCommonSetting;

        public override void Awake()
        {
            base.Awake();
            if (this != Inst) return;
            // Debug_CustomDataLoadAndSaveAll();
            // Debug_CustomDataLoadAndSaveJson();
            // Debug_JsonCustomDataLoadAndSave();
            //RegisterMessagePackResolver();
            LoadMenuSave();
            LoadArenaSave();
            LoadActionSetting();
            Application.targetFrameRate = 60;
        }


        private void Debug_CustomDataLoad(out List<(string path, CustomData data)> successList, out List<string> errorList)
        {
            var files = Directory.GetFiles($"{savePath}{Path.DirectorySeparatorChar}CustomData", $"*.xfc", SearchOption.AllDirectories);
            successList = new List<(string, CustomData)>();
            errorList = new List<string>();
            foreach (var file in files)
            {
                try
                {
                    using var dc = new BrotliDecompressor();
                    using var c = new BrotliCompressor();
                    var data = MemoryPackSerializer.Deserialize<CustomData>(dc.Decompress(File.ReadAllBytes(file)));
                    MemoryPackSerializer.Serialize(c, data);
                    File.WriteAllBytes(file, c.ToArray());
                    successList.Add((file, data));
                }
                catch (Exception e)
                {
                    errorList.Add(file);
                }
            }
            Debug.Log($"Success:{successList.Count}\n{ZString.Join("\n", successList)}");
            Debug.Log($"Error{errorList.Count}:\n{ZString.Join("\n", errorList)}");
        }

        /// <summary>
        /// デバッグ用メソッド。
        /// CustomDataを読み込み、再セーブしたい場合に使用する。
        /// </summary>
        /// <param name="excludingErrorData">読み込み再セーブの過程でエラーが発生したデータを除外フォルダに移動するフラグ。危険なのでデフォルトではオフにしておく。</param>
        private void Debug_CustomDataLoadAndSaveAll(bool excludingErrorData = false)
        {
            Debug_CustomDataLoad(out _, out var errorList);
            foreach (var file in errorList)
            {
                var destFileName = $"{savePath}{Path.DirectorySeparatorChar}Unreadable{Path.DirectorySeparatorChar}{Path.GetRelativePath(savePath, file)}";
                var destDir = Path.GetDirectoryName(destFileName);
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }
                File.Move(file, destFileName);
            }
        }
        private void Debug_CustomDataLoadAndSaveJson()
        {
            Debug_CustomDataLoad(out var successList, out _);
            foreach (var loaded in successList)
            {
                var json = SerializationUtility.SerializeValue(loaded.data, DataFormat.JSON);
                var destFileName = $"{savePath}{Path.DirectorySeparatorChar}JsonCustomData{Path.DirectorySeparatorChar}{Path.GetRelativePath(savePath, loaded.path)}";
                var destDir = Path.GetDirectoryName(destFileName);
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }
                File.WriteAllBytes(destFileName, json);
            }
        }
        private void Debug_JsonCustomDataLoadAndSave()
        {
            var jsonPath = $"{savePath}{Path.DirectorySeparatorChar}JsonCustomData";
            var filePaths = Directory.GetFiles(jsonPath, $"*.xfc", SearchOption.AllDirectories);
            foreach (var fp in filePaths)
            {
                var data = SerializationUtility.DeserializeValue<CustomData>(File.ReadAllBytes(fp), DataFormat.JSON);
                using var c = new BrotliCompressor();
                MemoryPackSerializer.Serialize(c, data);
                var destFileName = $"{savePath}{Path.DirectorySeparatorChar}{Path.GetRelativePath(jsonPath, fp)}";
                var destDir = Path.GetDirectoryName(destFileName);
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }
                File.WriteAllBytes(destFileName, c.ToArray());
            }
        }

        private void LoadMenuSave()
        {
            if (!File.Exists(menuSaveFileName)) return;
            byte[] bl = File.ReadAllBytes(menuSaveFileName);
            try
            {
                using var dc = new BrotliDecompressor();
                menuSaveData = MemoryPackSerializer.Deserialize<MenuSaveData>(dc.Decompress(bl));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void SaveMenuSave()
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            try
            {
                using var c = new BrotliCompressor();
                MemoryPackSerializer.Serialize(c, menuSaveData);
                File.WriteAllBytes(menuSaveFileName, c.ToArray());
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public void LoadArenaSave()
        {
            if (!File.Exists(arenaSaveFileName)) return;
            byte[] bl = File.ReadAllBytes(arenaSaveFileName);
            try
            {
                using var dc = new BrotliDecompressor();
                arenaSaveData = MemoryPackSerializer.Deserialize<ArenaSaveData>(dc.Decompress(bl));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public void SaveArenaSave()
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            try
            {
                using var c = new BrotliCompressor();
                MemoryPackSerializer.Serialize(c, arenaSaveData);
                File.WriteAllBytes(arenaSaveFileName, c.ToArray());
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void LoadActionSetting()
        {
            if (!File.Exists(actionSettingFileName)) return;
            byte[] bl = File.ReadAllBytes(actionSettingFileName);
            ActionSettingData = MemoryPackSerializer.Deserialize<ActionSettingData>(bl);
        }

        private void SaveActionSetting()
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            File.WriteAllBytes(actionSettingFileName, MemoryPackSerializer.Serialize(ActionSettingData));
        }

        public void NextScene(int num)
        {
            Scene now = SceneManager.GetActiveScene();
            sceneLog.Add(now.buildIndex);
            SaveMenuSave();
            SceneManager.LoadScene(num);
        }

        public void ReturnScene()
        {
            SaveMenuSave();
            SceneManager.LoadScene(sceneLog[^1]);

            sceneLog.RemoveAt(sceneLog.Count - 1);
        }

        public void SetUnityRandomSeedUseDateTimeNow()
        {
            unityRandomSeed = DateTime.Now.GetHashCode();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause) SaveMenuSave();
        }

        private void OnDestroy()
        {
            if (Inst == this)
            {
                SaveMenuSave();
                SaveActionSetting();
            }
        }
    }
}