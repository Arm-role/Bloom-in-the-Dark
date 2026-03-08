using System.Collections.Generic;
using System.Threading.Tasks;

public class AsyncObjectPool<T> where T : class
{
    private readonly Queue<T> pool = new();
    private readonly IAsyncGameObjectFactory<T> _factory;
    public AsyncObjectPool(IAsyncGameObjectFactory<T> factory)
    {
        _factory = factory;
    }
    public async Task<T> GetAsync()
    {
        T obj;
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            obj = await _factory.CreateAsync();
        }

        return obj;
    }

    public void Return(T item)
    {
        pool.Enqueue(item);
    }
}
