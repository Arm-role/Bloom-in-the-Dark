using System.Threading.Tasks;
using UnityEngine;

public class GameObjectSpawner : ISpawner
{
  private readonly IAdressablePoolService<GameObject> _poolService;
  private readonly GameObjectLibrary _gameObjectLibrary;

  public GameObjectSpawner(IAdressablePoolService<GameObject> poolService, GameObjectLibrary itemLibrary)
  {
    _poolService = poolService;
    _gameObjectLibrary = itemLibrary;
  }

  private async Task<GameObject> SpawnLocal(int id, Vector3 position)
  {
    var assetRef = _gameObjectLibrary.Find(id);

    if (assetRef == null) { Debug.Log($"Not Found {id}"); return null; }

    GameObject instance = await _poolService.AsyncGet(assetRef);
    instance.transform.position = position;

    var pooled = instance.GetComponent<IPooObject>();
    if (pooled == null)
      Debug.LogError("Object Not Have ID");

    pooled.Initialize(id);
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
    var key = Ob.GetComponent<IPooObject>().KeyId;

    if (key == 0)
    {
      Object.Destroy(Ob);
      Debug.Log($"DestroyObject Id : {key}");
      return;
    }

    var assetRef = _gameObjectLibrary.Find(key);

    if (assetRef != null)
    {
      _poolService.Return(assetRef, Ob);
    }

  }
}
