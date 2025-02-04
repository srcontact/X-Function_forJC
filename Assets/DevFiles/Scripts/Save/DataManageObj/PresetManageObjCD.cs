using clrev01.ClAction.Machines;

namespace clrev01.Save.DataManageObj
{
    [System.Serializable]
    public class PresetManageObjCD : PresetManageObj<CustomData>
    {
        public override string fileExt => ".xfc";
    }
}