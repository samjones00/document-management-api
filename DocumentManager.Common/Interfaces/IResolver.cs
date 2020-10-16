namespace DocumentManager.Common.Interfaces
{
    public interface IResolver<T>
    {
        T Resolve();
    }
}
