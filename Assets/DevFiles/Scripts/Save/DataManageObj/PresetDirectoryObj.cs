using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace clrev01.Save.DataManageObj
{
    [System.Serializable]
    public class PresetDirectoryObj
    {
        #region directryName

        [SerializeField]
        private string _directoryName;
        public string directoryName => _directoryName;

        #endregion

        #region nowDirName

        public string nowDirName => (parentDir != null ? parentDir.nowDirName + Path.DirectorySeparatorChar : "") + directoryName;

        #endregion

        public List<ScriptableTextAsset> presets = new();
        public List<PresetDirectoryObj> directories = new();
        [System.NonSerialized]
        public PresetDirectoryObj parentDir;

        public void InitializeOnUse(PresetDirectoryObj parent)
        {
            parentDir = parent;
            foreach (var d in directories)
            {
                d.InitializeOnUse(this);
            }
        }
    }
}