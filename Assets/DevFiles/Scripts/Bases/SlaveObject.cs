using UnityEngine;

namespace clrev01.Bases
{
    public abstract class SlaveObjectHD<C, L, H> : Hard<C, L, H>
        where C : SlaveObjectCD<C, L, H>
        where L : SlaveObjectLD<C, L, H>, new()
        where H : SlaveObjectHD<C, L, H>
    {
        protected abstract bool stillAlive { get; }

        public void Init(GameObject master)
        {
            ld.master = master;
        }
        public override void RunBeforePhysics()
        {
            if (ld.master != null && !ld.master.activeInHierarchy)
            {
                ld.master = null;
            }
            if (ld.master != null || stillAlive) return;
            Disable();
        }
        public override void OnDotonExe()
        { }
    }

    public abstract class SlaveObjectCD<C, L, H> : CommonData<C, L, H>
        where C : SlaveObjectCD<C, L, H>
        where L : SlaveObjectLD<C, L, H>, new()
        where H : SlaveObjectHD<C, L, H>
    { }

    public abstract class SlaveObjectLD<C, L, H> : LocalData<C, L, H>
        where C : SlaveObjectCD<C, L, H>
        where L : SlaveObjectLD<C, L, H>, new()
        where H : SlaveObjectHD<C, L, H>
    {
        public GameObject master;
    }
}