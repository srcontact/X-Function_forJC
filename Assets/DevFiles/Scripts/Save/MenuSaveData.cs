using clrev01.ClAction.Machines;
using MemoryPack;
using System.Collections.Generic;

namespace clrev01.Save
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class MenuSaveData : SaveData
    {
        public CustomData nowEditMech;

        public TeamData nowEditTeam;

        public MatchData testMatch = new MatchData();

        public List<List<bool>> testMatchNowEditMechNums;

        public TeamData arenaMyTeam;

        [MemoryPackOnSerializing]
        public void OnBeforeSerialize()
        {
            if (testMatch == null) return;
            // Debug.Log("ser");
            testMatchNowEditMechNums = new List<List<bool>>();
            // Debug.Log("ser_start:" + String.Join("/", testMatchNowEditMechNums.ConvertAll(x => String.Join(",", x))));

            for (var i = 0; i < testMatch.teamList.Count; i++)
            {
                testMatchNowEditMechNums.Add(new List<bool>());
                if (testMatch.teamList[i] == null) continue;
                foreach (var machine in testMatch.teamList[i].machineList)
                {
                    //todo:編集中データの保存方式を変更する。データ自体ではなくフラグやアドレスを保存するなど
                    testMatchNowEditMechNums[i].Add(ReferenceEquals(machine, nowEditMech));
                }
            }
            // Debug.Log("ser_end:" + String.Join("/", testMatchNowEditMechNums.ConvertAll(x => String.Join(",", x))));
        }

        [MemoryPackOnDeserialized]
        public void OnAfterDeserialize()
        {
            // Debug.Log("des");
            if (testMatch == null || testMatchNowEditMechNums == null) return;
            // Debug.Log("des_start:" + String.Join("/", testMatchNowEditMechNums.ConvertAll(x => String.Join(",", x))));
            for (int i = 0; i < testMatchNowEditMechNums.Count; i++)
            {
                if (testMatch.teamList.Count < i) continue;
                for (int j = 0; j < testMatchNowEditMechNums[i].Count; j++)
                {
                    if (testMatch.teamList[i].machineList.Count < j) continue;
                    if (testMatchNowEditMechNums[i][j]) testMatch.teamList[i].machineList[j] = nowEditMech;
                }
            }
            // Debug.Log("des_end:" + String.Join("/", testMatchNowEditMechNums.ConvertAll(x => String.Join(",", x))));
        }
    }
}