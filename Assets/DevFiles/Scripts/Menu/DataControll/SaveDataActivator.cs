using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.Extensions;
using clrev01.Save;
using UnityEngine;

namespace clrev01.Menu.DataControll
{
    public class SaveDataActivator : MenuFunction, IDataTransport<SaveData>
    {
        [SerializeField]
        DataManager dataManager;
        public SaveData tData
        {
            get
            {
                return StaticInfo.Inst.nowEditMech;
            }
            set
            {
                StaticInfo.Inst.nowEditMech = (CustomData)value;
            }
        }

        public override void ExeOnClick()
        {
            base.ExeOnClick();
            dataManager.SetSaveTgt(this, DataManager.DataMode.Custom);
            //MPPM.dialogManager.saveDialog.OpenSaveDialog(tData);
        }
    }
}