using MemoryPack;
using MemoryPack.Compression;
using System;
using System.IO;
using UnityEngine;

namespace clrev01.Save
{
    public abstract class Saver<D> : BaseOfSaver where D : SaveData, new()
    {
        #region filePath
        public virtual string filePathNameOnly
        {
            get
            {
                return null;
            }
        }
        public string filePath => folderPath + Path.DirectorySeparatorChar + dataName + "." + fileExt;
        public string folderPath
        {
            get
            {
#if UNITY_EDITOR
                return $"{Application.dataPath}{Path.DirectorySeparatorChar}SaveData{Path.DirectorySeparatorChar}{filePathNameOnly}";
#else
            return $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}SaveData{Path.DirectorySeparatorChar}{filePathNameOnly}";
#endif
            }
        }
        #endregion
        #region fileExt
        public virtual string fileExt
        {
            get { return ".dat"; }
        }
        #endregion
        #region data
        [SerializeField]
        private D _data;
        public D data
        {
            get
            {
                if (_data == null) return new D();
                return _data;
            }
            set { _data = value; }
        }
        #endregion
        public override SaveData baseData
        {
            get
            {
                return _data;
            }
        }

        [ContextMenu("DataSave")]
        public virtual void DataSave()
        {
            SaveExecute();
        }
        [ContextMenu("DataLoad")]
        public virtual void DataLoad()
        {
            LoadExecute();
        }

        public void SaveExecute()
        {
            SafeCreateDirectory(folderPath);
            try
            {
                using var c = new BrotliCompressor();
                MemoryPackSerializer.Serialize(c, data);
                var bytes = c.ToArray();
                File.WriteAllBytes(filePath, bytes);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public void LoadExecute()
        {
            if (!File.Exists(filePath))
            {
                SaveExecute();
            }
            try
            {
                using var dc = new BrotliDecompressor();
                data = MemoryPackSerializer.Deserialize<D>(dc.Decompress(File.ReadAllBytes(filePath)));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public DirectoryInfo SafeCreateDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                return null;
            }
            return Directory.CreateDirectory(path);
        }


        //#if UNITY_EDITOR
        public bool save, load;

        public virtual void OnValidate()
        {
            if (save) DataSave();
            else if (load) DataLoad();
            save = false;
            load = false;
        }
        //#endif
    }
}