using clrev01.Save;
using clrev01.Save.DataManageObj;
using MemoryPack;
using MemoryPack.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class FileManageObj<T> : IDataManageObj where T : SaveData
{
    public DirectoryInfo directoryInfo
    {
        get;
        protected set;
    }
    private readonly string _rootPathName;
    private string rootPathName => new DirectoryInfo(_rootPathName).FullName;
    public abstract string fileExt { get; }
    public List<string> subDirectoryNames { get; set; } = new();
    public List<string> fileNames { get; set; } = new();
    public string nowDirName => directoryInfo.FullName;
    private readonly List<DirectoryInfo> _subDirectories = new();
    private readonly List<FileInfo> _fileInfos = new();

    private readonly List<DirectoryInfo> _moveDirectories = new();
    private readonly List<FileInfo> _moveFiles = new();
    public bool isTopDir => directoryInfo.FullName == rootPathName;

    public bool isPreset => false;

    public FileManageObj(string root)
    {
        _rootPathName = root;
        directoryInfo = new DirectoryInfo(root);
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }
        ReloadDirectory();
    }


    public void ReloadDirectory()
    {
        _subDirectories.Clear();
        _subDirectories.AddRange(directoryInfo.EnumerateDirectories());
        _fileInfos.Clear();
        _fileInfos.AddRange(directoryInfo.EnumerateFiles("*" + fileExt));
        subDirectoryNames.Clear();
        for (int i = 0; i < _subDirectories.Count; i++)
        {
            subDirectoryNames.Add(_subDirectories[i].Name);
        }
        fileNames.Clear();
        for (int i = 0; i < _fileInfos.Count; i++)
        {
            fileNames.Add(_fileInfos[i].Name.Remove(_fileInfos[i].Name.Length - fileExt.Length));
        }
    }

    #region DirectoryControll
    public void LoadFolder(int tgtNum)
    {
        directoryInfo = tgtNum == -1 ? directoryInfo.Parent : _subDirectories[tgtNum];
        ReloadDirectory();
    }
    public void JumpFolder(string tgtDir)
    {
        if (!Directory.Exists(tgtDir)) return;
        directoryInfo = new DirectoryInfo(tgtDir);
        ReloadDirectory();
    }
    public string CreateFolder(string newDirName)
    {
        string err = null;
        try
        {
            directoryInfo.CreateSubdirectory(newDirName);
        }
        catch (Exception e)
        {
            err = "An error occured. The following folder could not be created. \n\t" +
                  newDirName;
            Debug.LogError(e);
        }
        ReloadDirectory();
        return err;
    }
    public string[] RenameFolder(int tgtNum, string newName)
    {
        string err1 = null, err2 = null;
        try
        {
            CopyFolder(_subDirectories[tgtNum].Parent, new List<DirectoryInfo>() { _subDirectories[tgtNum] });
        }
        catch (Exception e)
        {
            err1 = "An error occured. The following folder could not be renamed. \n\t" +
                   _subDirectories[tgtNum].Name + " -> " + newName;
            Debug.LogError(e);
        }
        if (err1 != null) return new string[] { err1, err2 };
        try
        {
            _subDirectories[tgtNum].Delete();
            _subDirectories.RemoveAt(tgtNum);
        }
        catch (Exception e)
        {
            err2 = "An error occured. The following folder could not be deleted. \n\t" +
                   _subDirectories[tgtNum].Name;
            Debug.LogError(e);
        }
        ReloadDirectory();
        return new string[] { err1, err2 };
    }
    public string DeleteFolder(int[] tgtNums)
    {
        string err = null;
        List<string> errFs = new List<string>();
        foreach (var tgtNum in tgtNums)
        {
            try
            {
                _subDirectories[tgtNum].Delete(true);
            }
            catch (Exception e)
            {
                errFs.Add(_subDirectories[tgtNum].Name);
                Debug.LogError(e);
            }
        }
        if (errFs.Count > 0)
        {
            err = errFs.Count == 1 ? "An error occured. The following folder could not be deleted." : "Multiple Errors Occured. The following folders could not be deleted.";
            foreach (var t in errFs)
            {
                err += "\n\t" + t;
            }
        }
        ReloadDirectory();
        return err;
    }
    public void SetMoveFolder(int[] tgts)
    {
        foreach (var tgt in tgts)
        {
            _moveDirectories.Add(_subDirectories[tgt]);
        }
    }
    public string[] ExecuteMoveFolder(int toDirNum = -100, bool isCopy = false)
    {
        DirectoryInfo toDir;
        if (toDirNum == -100) toDir = directoryInfo;
        else if (toDirNum < 0) toDir = directoryInfo.Parent;
        else toDir = _subDirectories[toDirNum];

        List<string> el1 = new List<string>();
        List<string> el2 = new List<string>();
        string err1, err2;
        err1 = err2 = null;

        foreach (var directory in _moveDirectories)
        {
            try
            {
                CopyFolder(toDir, _moveDirectories);
            }
            catch (Exception e)
            {
                el1.Add(directory.Name);
                Debug.LogError(e);
            }
        }
        if (!isCopy)
        {
            foreach (var directory in _moveDirectories)
            {
                try
                {
                    directory.Delete(true);
                }
                catch (Exception e)
                {
                    el2.Add(directory.Name);
                    Debug.LogError(e);
                }
            }
        }
        _moveDirectories.Clear();
        if (el1.Count > 0)
        {
            err1 = el1.Count == 1 ? "An error occured. The following folder could not be saved." : "Multiple Errors Occured. The following folders could not be saved.";
            foreach (var e in el1)
            {
                err1 += "\n\t" + e;
            }
        }
        if (el2.Count > 0)
        {
            err2 = el2.Count == 2 ? "An error occured. The following folder could not be deleted." : "Multiple Errors Occured. The following folders could not be deleted.";
            foreach (var e in el2)
            {
                err2 += "\n\t" + e;
            }
        }
        ReloadDirectory();
        return new string[] { err1, err2 };
    }

    private void CopyFolder(DirectoryInfo toDir, List<DirectoryInfo> origFolders)
    {
        foreach (var origFolder in origFolders)
        {
            List<DirectoryInfo> dl =
                new List<DirectoryInfo>(origFolder.EnumerateDirectories("*", SearchOption.AllDirectories));
            dl.Insert(0, origFolder);
            List<FileInfo> fl =
                new List<FileInfo>(origFolder.EnumerateFiles("*", SearchOption.AllDirectories));
            foreach (var d in dl)
            {
                string subPath = toDir.FullName + d.FullName.Remove(0, origFolder.Parent.FullName.Length);
                toDir.CreateSubdirectory(subPath);
            }
            foreach (var f in fl)
            {
#if UNITY_EDITOR
                if (f.Extension == ".meta" || f.Extension == ".gitkeep") continue;
#endif
                string subPath = toDir.FullName + f.FullName.Remove(0, origFolder.Parent.FullName.Length);
                f.CopyTo(subPath, true);
            }
        }
    }
    #endregion

    #region FileControll
    public SaveData LoadFile(int loadNum, ref string err)
    {
        try
        {
            var bytes = File.ReadAllBytes(_fileInfos[loadNum].FullName);
            using var dc = new BrotliDecompressor();
            var t = MemoryPackSerializer.Deserialize<T>(dc.Decompress(bytes));
            if (t != null)
            {
                t.fileName = fileNames[loadNum];
            }
            return t;
        }
        catch (Exception e)
        {
            err = "An error occured. The file could not be loaded.";
            Debug.LogError(e);
            return null;
        }
    }
    public string SaveFile(SaveData saveData)
    {
        string err = null;
        try
        {
            using var c = new BrotliCompressor();
            MemoryPackSerializer.Serialize(c, (T)saveData);
            var bytes = c.ToArray();
            File.WriteAllBytes(directoryInfo.FullName + Path.DirectorySeparatorChar + saveData.fileName + fileExt, bytes);
        }
        catch (Exception e)
        {
            err = "An error occured. The following file could not be saved. \n\t" +
                  saveData.fileName + fileExt;
            Debug.LogError(e);
        }
        ReloadDirectory();
        return err;
    }
    public string[] RenameFile(int tgtNum, string newName)
    {
        string err1, err2;
        err1 = err2 = null;
        try
        {
            _fileInfos[tgtNum].CopyTo(directoryInfo.FullName + Path.DirectorySeparatorChar + newName + fileExt, true);
        }
        catch (Exception e)
        {
            err1 = "An error occured. The following file could not be renamed. \n\t" +
                   _fileInfos[tgtNum].Name + " -> " + newName + fileExt;
            Debug.LogError(e);
        }
        if (err1 != null) return new string[] { err1, err2 };
        try
        {
            _fileInfos[tgtNum].Delete();
            _fileInfos.RemoveAt(tgtNum);
        }
        catch (Exception e)
        {
            err2 = "An error occured. The following file could not be deleted. \n\t" +
                   _fileInfos[tgtNum].Name;
            Debug.LogError(e);
        }
        ReloadDirectory();
        return new string[] { err1, err2 };
    }
    public string DeleteFiles(int[] tgtNums)
    {
        string err = null;
        List<string> errFs = new List<string>();
        foreach (var tgtNum in tgtNums)
        {
            try
            {
                _fileInfos[tgtNum].Delete();
            }
            catch (Exception e)
            {
                errFs.Add(_fileInfos[tgtNum].Name);
                Debug.LogError(e);
            }
        }
        if (errFs.Count > 0)
        {
            err = errFs.Count == 1 ? "An error occured. The following file could not be deleted." : "Multiple Errors Occured. The following files could not be deleted.";
            foreach (var errF in errFs)
            {
                err += "\n\t" + errF;
            }
        }
        ReloadDirectory();
        return err;
    }
    public void SetMoveFiles(int[] tgts)
    {
        foreach (var tgt in tgts)
        {
            _moveFiles.Add(_fileInfos[tgt]);
        }
    }
    public string[] ExecuteMoveFiles(int toDirNum = -100, bool isCopy = false)
    {
        DirectoryInfo toDir;
        if (toDirNum == -100) toDir = directoryInfo;
        else if (toDirNum < 0) toDir = directoryInfo.Parent;
        else toDir = _subDirectories[toDirNum];

        List<string> el1 = new List<string>();
        List<string> el2 = new List<string>();
        string err1, err2;
        err1 = err2 = null;

        foreach (var moveFile in _moveFiles)
        {
            try
            {
                moveFile.CopyTo(toDir.FullName + Path.DirectorySeparatorChar + moveFile.Name, true);
            }
            catch (Exception e)
            {
                el1.Add(moveFile.Name);
                Debug.LogError(e);
                continue;
            }
            if (!isCopy)
            {
                try
                {
                    moveFile.Delete();
                }
                catch (Exception e)
                {
                    el2.Add(moveFile.Name);
                    Debug.LogError(e);
                }
            }
        }
        _moveFiles.Clear();
        if (el1.Count > 0)
        {
            err1 = el1.Count == 1 ? "An error occured. The following file could not be saved." : "Multiple Errors Occured. The following files could not be saved.";
            foreach (var e in el1)
            {
                err1 += "\n\t" + e;
            }
        }
        if (el2.Count > 0)
        {
            err2 = el2.Count == 2 ? "An error occured. The following file could not be deleted." : "Multiple Errors Occured. The following files could not be deleted.";
            foreach (var e in el2)
            {
                err2 += "\n\t" + e;
            }
        }
        ReloadDirectory();
        return new string[] { err1, err2 };
    }
    #endregion
    public void ResetMoveLists()
    {
        _moveDirectories.Clear();
        _moveFiles.Clear();
    }

    public List<bool> GetMoveDirsInNowDir()
    {
        List<bool> bl = new List<bool>();
        foreach (var subDirectory in _subDirectories)
        {
            bool b = false;
            foreach (var moveDirectory in _moveDirectories)
            {
                if (!moveDirectory.FullName.Equals(subDirectory.FullName)) continue;
                b = true;
                break;
            }

            bl.Add(b);
        }
        return bl;
    }
    public List<bool> GetMoveFilesInNowDir()
    {
        List<bool> bl = new List<bool>();
        foreach (var fileInfo in _fileInfos)
        {
            bool b = false;
            foreach (var moveFile in _moveFiles)
            {
                if (!moveFile.FullName.Equals(fileInfo.FullName)) continue;
                b = true;
                break;
            }

            bl.Add(b);
        }
        return bl;
    }
    public List<string> GetMoveFileNames(string parentD)
    {
        List<string> paths = new List<string>();
        foreach (var moveFile in _moveFiles)
        {
            paths.Add(parentD + "\\" + moveFile.Name);
        }
        foreach (var moveDirectory in _moveDirectories)
        {
            List<FileInfo> fl =
                new List<FileInfo>(moveDirectory.EnumerateFiles("*", SearchOption.AllDirectories));
            foreach (var f in fl)
            {
                if (f.Extension != fileExt) continue;
                paths.Add(parentD + "\\" + f.FullName.Remove(0, moveDirectory.Parent.FullName.Length));
            }
        }
        return paths;
    }
}