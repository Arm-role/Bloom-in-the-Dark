using System.Collections;
using UnityEngine;

public class EnemyPatternBrain : MonoBehaviour
{
  private Coroutine _running;
  public EnemyPattern pattern;
  public bool IsRunning => _running != null;

  public void Tick(EnemyController c)
  {
    if (_running != null || pattern == null) return;
    _running = StartCoroutine(RunAndClear(c));
  }

  private IEnumerator RunAndClear(EnemyController c)
  {
    yield return pattern.Run(c);
    _running = null;
  }

  public void StopPattern()
  {
    if (_running == null) return;
    StopCoroutine(_running);
    _running = null;
  }
}