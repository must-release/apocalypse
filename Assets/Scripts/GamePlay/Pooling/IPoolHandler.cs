
public interface IPoolHandler<TPoolObject> where TPoolObject : IPoolable
{
    TPoolObject Get();
    void Return(TPoolObject poolObject);
}