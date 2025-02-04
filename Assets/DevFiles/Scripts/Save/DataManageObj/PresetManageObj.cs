using MemoryPack;
using MemoryPack.Compression;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace clrev01.Save.DataManageObj
{
    public abstract class PresetManageObj<T> : IDataManageObj where T : SaveData
    {
        public PresetDirectoryObj presetDirectoryObj;
        #region selected
        private PresetDirectoryObj _selected;
        public PresetDirectoryObj selected
        {
            get
            {
                if (_selected == null) _selected = presetDirectoryObj;
                return _selected;
            }
            set => _selected = value;
        }
        #endregion
        public virtual string fileExt { get; }
        public List<string> subDirectoryNames { get; set; } = new();
        public List<string> fileNames { get; set; } = new();
        public string nowDirName => selected.nowDirName;
        public bool isTopDir => selected == presetDirectoryObj;

        public bool isPreset => true;

        public void ReloadDirectory()
        {
            subDirectoryNames.Clear();
            for (int i = 0; i < selected.directories.Count; i++)
            {
                subDirectoryNames.Add(selected.directories[i].directoryName);
            }
            fileNames.Clear();
            for (int i = 0; i < selected.presets.Count; i++)
            {
                fileNames.Add(selected.presets[i].name);
            }
        }
        public void LoadFolder(int tgtNum)
        {
            selected = tgtNum < 0 ? selected.parentDir : selected.directories[tgtNum];
        }
        public SaveData LoadFile(int loadNum, ref string err)
        {
            try
            {
                using var dc = new BrotliDecompressor();
                return MemoryPackSerializer.Deserialize<T>(dc.Decompress(selected.presets[loadNum].bytes));
            }
            catch (Exception e)
            {
                err = "An error occured. The preset could not be loaded.";
                Debug.LogError(e);
                return null;
            }
        }

        public void InitializeOnUse()
        {
            selected = presetDirectoryObj;
            selected.InitializeOnUse(null);
            ReloadDirectory();
        }

        #region 実装しない
        public void JumpFolder(string tgtDir)
        {
            throw new System.NotImplementedException();
        }

        public string CreateFolder(string newDirName)
        {
            throw new System.NotImplementedException();
        }

        public string[] RenameFolder(int tgtNum, string newName)
        {
            throw new System.NotImplementedException();
        }

        public string DeleteFolder(int[] delDirNums)
        {
            throw new System.NotImplementedException();
        }

        public void SetMoveFolder(int[] tgts)
        {
            throw new System.NotImplementedException();
        }

        public string[] ExecuteMoveFolder(int toDirNum = -100, bool isCopy = false)
        {
            throw new System.NotImplementedException();
        }

        public string SaveFile(SaveData saveData)
        {
            throw new System.NotImplementedException();
        }

        public string[] RenameFile(int tgtNum, string newName)
        {
            throw new System.NotImplementedException();
        }

        public string DeleteFiles(int[] tgtNums)
        {
            throw new System.NotImplementedException();
        }

        public void SetMoveFiles(int[] tgts)
        {
            throw new System.NotImplementedException();
        }

        public string[] ExecuteMoveFiles(int toDirNum = -100, bool isCopy = false)
        {
            throw new System.NotImplementedException();
        }

        public void ResetMoveLists()
        {
            throw new System.NotImplementedException();
        }

        public List<bool> GetMoveDirsInNowDir()
        {
            throw new System.NotImplementedException();
        }

        public List<bool> GetMoveFilesInNowDir()
        {
            throw new System.NotImplementedException();
        }

        public List<string> GetMoveFileNames(string parentD)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}