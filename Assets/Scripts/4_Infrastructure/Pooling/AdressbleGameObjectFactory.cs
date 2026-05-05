using System.Threading.Tasks;
using UnityEngine;

public class AdressbleGameObjectFactory : IAsyncGameObjectFactory<GameObject>
{
    private readonly GameObject _prefab;
    private readonly Transform _parent;

    public AdressbleGameObjectFactory(GameObject prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }

    public Task<GameObject> CreateAsync()
    {
        GameObject instance = Object.Instantiate(_prefab, _parent);
        return Task.FromResult(instance); 
    }
}