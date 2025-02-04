using clrev01.Bases;
using UnityEngine;

namespace clrev01.Settings
{
    [CreateAssetMenu(menuName = "BaseSettings/UIBaseSettings")]
    public class UIBaseSettings : SOBaseOfCL
    {
        #region doubleClickFrame
        [SerializeField]
        private int _doubleClickFrame = 10;
        public int doubleClickFrame
        {
            get { return _doubleClickFrame; }
        }
        #endregion
        #region nameChangeFrame
        [SerializeField]
        private int _nameChangeFrame = 60;
        public int nameChangeFrame
        {
            get { return _nameChangeFrame; }
            set { _nameChangeFrame = value; }
        }
        #endregion
    }
}