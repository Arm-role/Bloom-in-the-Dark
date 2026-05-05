using System.Collections.Generic;
using UnityEngine;

public class CrowdingTracker : MonoBehaviour
{
  public static CrowdingTracker Instance { get; private set; }

  // target → จำนวน enemy ที่กำลังไล่/โจมตีอยู่
  private Dictionary<Transform, int> _attackerCount = new();

  private void Awake()
  {
    if (Instance == null) Instance = this;
    else Destroy(gameObject);
  }

  public void Register(Transform target)
  {
    if (target == null) return;
    _attackerCount.TryGetValue(target, out int cur);
    _attackerCount[target] = cur + 1;
  }

  public void Unregister(Transform target)
  {
    if (target == null) return;
    if (!_attackerCount.TryGetValue(target, out int cur)) return;
    int next = Mathf.Max(0, cur - 1);
    if (next == 0) _attackerCount.Remove(target);
    else _attackerCount[target] = next;
  }

  public int GetAttackerCount(Transform target)
  {
    if (target == null) return 0;
    _attackerCount.TryGetValue(target, out int count);
    return count;
  }
}