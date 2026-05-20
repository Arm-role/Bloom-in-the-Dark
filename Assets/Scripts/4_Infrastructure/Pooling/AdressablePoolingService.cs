using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class AdressablePoolingService : IAdressablePoolService<GameObject>
{
  private readonly Dictionary<GameObject, AsyncObjectPool<GameObject>> _pool = new();

  // instance ที่ถูก Get ออกไปแล้วยังไม่ถูก Return — map ไป prefab เพื่อใช้ตอน ReturnAll
  private readonly Dictionary<GameObject, GameObject> _activeInstances = new();

  private readonly Transform _poolParent;

  public AdressablePoolingService(string gameObjectName = "[AdressablePoolingService_Root]")
  {
    _poolParent = new GameObject(gameObjectName).transform;
    Object.DontDestroyOnLoad(_poolParent.gameObject);
  }

  public async Task<GameObject> AsyncGet(GameObject prefab)
  {
    if (!_pool.TryGetValue(prefab, out var pool))
    {
      var factory = new AdressbleGameObjectFactory(prefab, _poolParent);
      pool = new AsyncObjectPool<GameObject>(factory);
      _pool.Add(prefab, pool);
    }

    var instance = await pool.GetAsync();
    _activeInstances[instance] = prefab;
    return instance;
  }
  public void Return(GameObject prefab, GameObject instance)
  {
    _activeInstances.Remove(instance);

    if (_pool.TryGetValue(prefab, out var pool))
    {
      instance.SetActive(false);
      pool.Return(instance);
    }
    else
    {
      Debug.LogWarning($"Trying to return object to a non-existent pool: {prefab.name}. Destroying instead.");
      Object.Destroy(instance);
    }
  }

  // คืน instance ที่ active ทุกตัวเข้า pool — เรียกตอนเข้า scene ใหม่
  // object ที่ไม่ถูก Despawn ปกติ (projectile/VFX/skill ที่ค้างกลางอากาศ) เป็นลูกของ
  // _poolParent (DDOL) จะตามข้าม scene มา — ReturnAll ดึงกลับ pool ให้ reuse ได้
  public void ReturnAll()
  {
    // snapshot — Return แก้ _activeInstances ระหว่าง iterate
    foreach (var pair in _activeInstances.ToList())
    {
      if (pair.Key != null)
        Return(pair.Value, pair.Key);
      else
        _activeInstances.Remove(pair.Key);
    }
  }
}
