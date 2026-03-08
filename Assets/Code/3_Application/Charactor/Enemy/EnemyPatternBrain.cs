using UnityEngine;

public class EnemyPatternBrain : MonoBehaviour
{
    public EnemyPattern pattern;

    private Coroutine _running;

    public bool Tick(EnemyController c, Transform target)
    {
        if (_running == null)
        {
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
