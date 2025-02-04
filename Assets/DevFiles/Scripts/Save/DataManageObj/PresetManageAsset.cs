using clrev01.Bases;
using UnityEngine;

namespace clrev01.Save.DataManageObj
{
    public class PresetManageAsset<O, T> : SOBaseOfCL
        where O : PresetManageObj<T>
        where T : SaveData
    {
        [SerializeField]
        private O _asset;
        public O asset
        {
            get { return _asset; }
        }
    }
}