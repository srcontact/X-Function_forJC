using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.Extensions;
using clrev01.Save;
using UnityEngine;

namespace clrev01.Menu.DataControll
{
    public class MachineLoadFunction : MenuFunction, IDataTransport<SaveData>
    {
        [SerializeField]
        DataManager dataManager;
        public SaveData tData
        {
            get => StaticInfo.Inst.nowEditMech;
            set => StaticInfo.Inst.nowEditMech = (CustomData)value;
        }
        [SerializeField]
        private bool isPreset;

        public override void ExeOnClick()
        {
            base.ExeOnClick();
            dataManager.SetLoadTgt(this, DataManager.DataMode.Custom, isPreset, false);
        }
    }
}