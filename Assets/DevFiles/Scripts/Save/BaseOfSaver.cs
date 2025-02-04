using clrev01.Bases;

namespace clrev01.Save
{
    public abstract class BaseOfSaver : SOBaseOfCL
    {
        public virtual string dataName
        {
            get { return "testData"; }
        }
        public abstract SaveData baseData { get; }
    }
}