namespace clrev01.Extensions
{
    public interface IDataTransport<T>
    {
        T tData
        {
            get;
            set;
        }
    }
}