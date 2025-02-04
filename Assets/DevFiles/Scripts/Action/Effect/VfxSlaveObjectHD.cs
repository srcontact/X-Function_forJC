using clrev01.Bases;

namespace clrev01.ClAction.Effect
{
    public class VfxSlaveObjectHD : SlaveObjectHD<VfxSlaveObjectCD, VfxSlaveObjectLD, VfxSlaveObjectHD>
    {
        public VfxControl vfxController;

        protected override bool stillAlive => vfxController.stillAliveParticle;
        public override void RunAfterPhysics()
        {
            if (ld.master == null && stillAlive) vfxController.VfxStop();
        }
        public override void RunOnAfterFixedUpdateAndAnimation()
        {
            base.RunOnAfterFixedUpdateAndAnimation();
            if (ld.master) transform.SetPositionAndRotation(ld.master.transform.position, ld.master.transform.rotation);
        }
    }
}