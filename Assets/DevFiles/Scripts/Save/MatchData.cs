using MemoryPack;
using System.Collections.Generic;
using System.Linq;

namespace clrev01.Save
{
    [System.Serializable]
    [MemoryPackable()]
    public partial class MatchData : SaveData
    {
        [MemoryPackIgnore]
        public int MaxTeamNum => 12;

        public List<TeamData> teamList = new();
        public bool fixedRandomSeed;
        public int randomSeed = 0;
        public int playLevelNum;
        public int levelSizeNumber = 1;
        public int actionEndFrame = 120 * 60;

        /// <summary>
        /// 試合開始可能か？
        /// </summary>
        public bool startPossible
        {
            get
            {
                return teamList != null && teamList.Count(x => x != null && x.machineList.Count(y => y != null) > 0) > 1;
            }
        }
        public int machineNumSum
        {
            get
            {
                var count = 0;
                foreach (var team in teamList)
                {
                    count += team.machineList.Count;
                }
                return count;
            }
        }

        public void ClearEmptyTeam()
        {
            for (int i = teamList.Count - 1; i >= 0; i--)
            {
                if (teamList[i].machineList.Count < 1) teamList.RemoveAt(i);
            }
        }
        public bool AddNewTeam(string newTeamName = "")
        {
            if (teamList.Count >= MaxTeamNum) return false;

            teamList.Add(new TeamData());
            if (newTeamName == "") newTeamName = GetSerialNumberName("Team");
            teamList[teamList.Count - 1].dataName = newTeamName;
            return true;
        }
        public string GetSerialNumberName(string template)
        {
            string s;
            bool b = false;
            for (int j = 0; j < MaxTeamNum; j++)
            {
                s = template + (j + 1).ToString("00");
                for (int i = 0; i < teamList.Count; i++)
                {
                    if (s.Equals(teamList[i].dataName))
                    {
                        b = true;
                        break;
                    }
                }
                if (!b) return s;
                else b = false;
            }
            return template;
        }
    }
}