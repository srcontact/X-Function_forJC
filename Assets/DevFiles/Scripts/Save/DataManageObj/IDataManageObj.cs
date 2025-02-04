using System.Collections.Generic;

namespace clrev01.Save.DataManageObj
{
    public interface IDataManageObj
    {
        public bool isPreset { get; }
        public string fileExt { get; }
        public List<string> subDirectoryNames { get; set; }
        public List<string> fileNames { get; set; }
        public string nowDirName { get; }
        public bool isTopDir { get; }

        public void ReloadDirectory();
        public void LoadFolder(int tgtNum);
        public void JumpFolder(string tgtDir);
        public string CreateFolder(string newDirName);
        public string[] RenameFolder(int tgtNum, string newName);
        public string DeleteFolder(int[] delDirNums);
        public void SetMoveFolder(int[] tgts);
        public string[] ExecuteMoveFolder(int toDirNum = -100, bool isCopy = false);
        public SaveData LoadFile(int loadNum, ref string err);
        public string SaveFile(SaveData saveData);
        public string[] RenameFile(int tgtNum, string newName);
        public string DeleteFiles(int[] tgtNums);
        public void SetMoveFiles(int[] tgts);
        public string[] ExecuteMoveFiles(int toDirNum = -100, bool isCopy = false);

        public void ResetMoveLists();

        public List<bool> GetMoveDirsInNowDir();
        public List<bool> GetMoveFilesInNowDir();
        public List<string> GetMoveFileNames(string parentD);
    }
}