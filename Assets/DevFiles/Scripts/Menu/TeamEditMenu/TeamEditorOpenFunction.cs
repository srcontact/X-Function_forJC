using clrev01.Bases;
using clrev01.Extensions;
using clrev01.Save;
using UnityEngine;

namespace clrev01.Menu.TeamEditMenu
{
    public class TeamEditorOpenFunction : MenuFunction, IDataTransport<SaveData>
    {
        [SerializeField]
        private TeamEditManager _teamEditManager;
        public SaveData tData
        {
            get => StaticInfo.Inst.nowEditTeam;
            set => StaticInfo.Inst.nowEditTeam = (TeamData)value;
        }

        public override void ExeOnClick()
        {
            base.ExeOnClick();
            _teamEditManager.isTestDataEdit = false;
            _teamEditManager.SetEditTgt(this);
        }
    }
}