using System.Threading.Tasks;
using UnityEngine;

public class GameObjectSpawner
{
    private readonly IAdressablePoolService<GameObject> _poolService;
    private readonly GameObjectLibrary _gameObjectLibrary;

    public GameObjectSpawner(IAdressablePoolService<GameObject> poolService, GameObjectLibrary itemLibrary)
    {
        _poolService = poolService;
        _gameObjectLibrary = itemLibrary;
    }

    private async Task<GameObject> SpawnLocal(string itemName, Vector3 position)
    {
        var assetRef = _gameObjectLibrary.Find(itemName);

        if (assetRef == null) { Debug.Log($"Not Found {itemName}"); return null; }

        GameObject instance = await _poolService.AsyncGet(assetRef);
        instance.name = itemName;
        instance.transform.position = position;
        return instance;
    }

    private async Task<GameObject> SpawnLocal(int id, Vector3 position)
    {
        var assetRef = _gameObjectLibrary.Find(id);

        if (assetRef == null) { Debug.Log($"Not Found {id}"); return null; }

        GameObject instance = await _poolService.AsyncGet(assetRef);
        instance.transform.position = position;
        return instance;
    }

    public async Task<GameObject> SpawnAsync(string itemName, Vector3 position)
    {
        GameObject instance = await SpawnLocal(itemName, position);

        instance.SetActive(true);
        return instance;
    }
    public async Task<GameObject> SpawnAsync(string itemName, Vector3 position, Vector3 direction)
    {
        GameObject instance = await SpawnLocal(itemName, position);

        if (direction.sqrMagnitude > 0.0001f)
        {
            instance.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction.normalized);
        }
        else
        {
            instance.transform.rotation = Quaternion.identity;
        }

        instance.SetActive(true);
        return instance;
    }

    public async Task<GameObject> SpawnAsync(int id, Vector3 position)
    {
        GameObject instance = await SpawnLocal(id, position);

        instance.SetActive(true);
        return instance;
    }

    public async Task<GameObject> SpawnAsync(int id, Vector3 position, Vector3 direction)
    {
        GameObject instance = await SpawnLocal(id, position);

        if (direction.sqrMagnitude > 0.0001f)
        {
            instance.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction.normalized);
        }
        else
        {
            instance.transform.rotation = Quaternion.identity;
        }

        instance.SetActive(true);
        return instance;
    }
    public void Despawn(GameObject Ob)
    {
        var assetRef = _gameObjectLibrary.Find(Ob.name);

        if (assetRef != null)
        {
            _poolService.Return(assetRef, Ob);
        }
    }
}
