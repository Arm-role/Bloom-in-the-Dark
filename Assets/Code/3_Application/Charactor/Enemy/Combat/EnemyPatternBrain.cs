using UnityEngine;

public class EnemyPatternBrain : MonoBehaviour
{
  private Coroutine _running;
  public EnemyPattern pattern;
  public bool IsRunning => _running != null;

  public bool Tick(EnemyController c, Transform target)
  {
    Debug.Log($"[PatternBrain] Tick — pattern={pattern}, running={_running != null}");

    if (_running == null)
    {
      if (pattern == null)
      {
        Debug.LogError("[PatternBrain] pattern is NULL — ไม่ได้ assign ScriptableObject ใน Inspector");
        return false;
      }

      _running = StartCoroutine(pattern.Run(c, target));
      return false;
    }

    return false;
  }

  public void StopPattern()
  {
    if (_running != null)
    {
      StopCoroutine(_running);
      _running = null;
    }
  }
}
