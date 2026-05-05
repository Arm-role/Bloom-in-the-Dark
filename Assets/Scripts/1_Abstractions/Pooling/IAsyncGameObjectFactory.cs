using System.Threading.Tasks;

public interface IAsyncGameObjectFactory<T>
{
    Task<T> CreateAsync();
}
