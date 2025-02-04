using Cysharp.Text;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace clrev01.Save
{
    [System.Serializable]
    public class PathLink
    {
        public string directoryName;
        public int selectedSubPathLink = -1;
        public List<PathLink> subPathLinks = new List<PathLink>();

        public PathLink(string name = "")
        {
            directoryName = name;
        }

        /// <summary>
        /// 現在のディレクトリのパスを取得する。
        /// </summary>
        /// <param name="fp"></param>
        public void GetFolderPath(ref Utf8ValueStringBuilder fp)
        {
            fp.Append(directoryName);
            fp.Append(Path.DirectorySeparatorChar);
            if (selectedSubPathLink == -1) return;
            subPathLinks[selectedSubPathLink].GetFolderPath(ref fp);
        }
        /// <summary>
        /// フォルダーの構造を登録する。
        /// </summary>
        /// <param name="previousDir"></param>
        public void SettingSubFolders(string previousDir = "")
        {
            subPathLinks.Clear();

            DirectoryInfo dir = new DirectoryInfo(previousDir + directoryName);
            List<DirectoryInfo> subFolderPaths = new List<DirectoryInfo>();
            subFolderPaths.AddRange(dir.GetDirectories("*", SearchOption.TopDirectoryOnly));
            foreach (DirectoryInfo d in subFolderPaths)
            {
                PathLink link = new PathLink(d.Name);
                subPathLinks.Add(link);
                //link.directoryName = d.Name;
                link.SettingSubFolders(dir.FullName + Path.DirectorySeparatorChar);
            }
        }

        public void SelectFolder(int num)
        {
            if (selectedSubPathLink == -1)
            {
                selectedSubPathLink = num;
                return;
            }
            else
            {
                subPathLinks[selectedSubPathLink].SelectFolder(num);
            }
        }
        public void ReturnFolder()
        {
            if (subPathLinks[selectedSubPathLink].selectedSubPathLink == -1)
            {
                selectedSubPathLink = -1;
            }
            else subPathLinks[selectedSubPathLink].ReturnFolder();
        }

        /// <summary>
        /// 選択中のフォルダのPathLinkを返す。
        /// </summary>
        /// <returns></returns>
        public PathLink GetNowPathLink()
        {
            if (selectedSubPathLink == -1)
            {
                return this;
            }
            else
            {
                return subPathLinks[selectedSubPathLink].GetNowPathLink();
            }
        }

        /// <summary>
        /// フォルダ選択をリセットする。
        /// </summary>
        public void ResetFolderSelect()
        {
            selectedSubPathLink = -1;
            foreach (PathLink sp in subPathLinks)
            {
                sp.ResetFolderSelect();
            }
        }
    }
}