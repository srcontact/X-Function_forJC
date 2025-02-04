using UnityEngine;

namespace clrev01.Bases
{
    public abstract class CommonDataBase : SOBaseOfCL
    {
        [SerializeField]
        private Sprite _uiIcon;
        public Sprite uiIcon => _uiIcon == null ? StaticInfo.Inst.defaultIcon : _uiIcon;
        public abstract HardBase InstActorH(Vector3 position, Quaternion rotation);
        public abstract void StandbyPoolActors(int standbyNum);
    }
}