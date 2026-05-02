using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
  public static EnemyManager Instance { get; private set; }
  private readonly List<EnemyController> _enemies = new List<EnemyController>();

  private void Awake()
  {
    if (Instance != null && Instance != this) Destroy(this.gameObject);
    Instance = this;
  }

  public void RegisterEnemy(EnemyController e) { if (!_enemies.Contains(e)) _enemies.Add(e); }
  public void UnregisterEnemy(EnemyController e) { _enemies.Remove(e); }

  public List<EnemyController> QueryRadius(Vector2 pos, float radius)
  {
    float rsq = radius * radius;
    var outList = new List<EnemyController>();
    for (int i = 0; i < _enemies.Count; i++)
    {
      var e = _enemies[i];
      if (e == null) continue;
      if ((e.transform.position - (Vector3)pos).sqrMagnitude <= rsq) outList.Add(e);
    }
    return outList;
  }

  public void NotifyTargetDestroyed(Transform target)
  {
    foreach (var enemy in _enemies)
      enemy.OnTargetLost(target);
  }
}
