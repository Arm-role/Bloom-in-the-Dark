using System.Collections;
using UnityEngine;

public class EnemyPatternBrain : MonoBehaviour
{
  private Coroutine _running;
  private EnemyPattern _pattern;
  public bool IsRunning => _running != null;
  
  public void SetPattern(EnemyPattern pattern)
  {
    _pattern = pattern;
  }

  public void Tick(EnemyController c)
  {
    if (_running != null || _pattern == null) return;
    _running = StartCoroutine(RunAndClear(c));
  }

  private IEnumerator RunAndClear(EnemyController c)
  {
    yield return _pattern.Run(c);
    _running = null;
  }

  public void StopPattern()
  {
    if (_running == null) return;
    StopCoroutine(_running);
    _running = null;
  }
}