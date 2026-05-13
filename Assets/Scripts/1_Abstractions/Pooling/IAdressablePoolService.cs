using System.Threading.Tasks;
using UnityEngine;

public interface IAdressablePoolService<T>
{
    Task<GameObject> AsyncGet(T prefab);

    void Return(T prefab, GameObject instance);
}
