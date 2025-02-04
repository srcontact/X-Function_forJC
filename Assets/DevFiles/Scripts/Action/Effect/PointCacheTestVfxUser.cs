using clrev01.Bases;

namespace clrev01.ClAction.Effect
{
    public class PointCacheTestVfxUser : BaseOfCL
    {
        private PointCacheTestVfxControl _vfxControl;

        public void OnSpawn(PointCacheTestVfxControl vfxControl)
        {
            _vfxControl = vfxControl;
            _vfxControl.RegisterUser(this);
        }
        private void OnDisable()
        {
            _vfxControl.UnregisterUser(this);
        }
    }
}