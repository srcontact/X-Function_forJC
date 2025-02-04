using clrev01.Save;

namespace clrev01.Menu.TeamEditMenu
{
    public class TeamInformation : DataInformation<TeamData>
    {
        public override string emptyText => "/// EMPTY TEAM ///";

        public override string infoText
        {
            get
            {
                string s = indicateData.dataName;
                for (int i = 0; i < indicateData.machineList.Count; i++)
                {
                    if (indicateData.machineList[i] == null) continue;
                    s += "\n\t" + indicateData.machineList[i].dataName;
                }
                return s;
            }
        }
    }
}