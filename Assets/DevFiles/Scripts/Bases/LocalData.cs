namespace clrev01.Bases
{
    [System.Serializable]
    public class LocalData<C, L, H> : LocalDataBase
        where C : CommonData<C, L, H>
        where L : LocalData<C, L, H>, new()
        where H : Hard<C, L, H>
    {
        public C cd;
        public H hd;
        public override CommonDataBase cdb => cd;
        public override HardBase hdb => hd;

        public virtual void ResetEveryFrame()
        { }
        public virtual void RunBeforePhysics()
        { }
        public virtual void RunAfterPhysics()
        { }
    }
}