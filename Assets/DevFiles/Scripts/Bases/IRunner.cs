namespace clrev01.Bases
{
    public interface IRunner
    {
        void RunOnUpdate();
        void RunBeforePhysics();
        void RunAfterPhysics();
    }
}