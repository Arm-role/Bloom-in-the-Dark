using UnityEngine;

public class EnemySensor : MonoBehaviour
{
    public float detectionRadius = 6f;
    public float chaseRadius = 10f;
    public float attackRadius = 1.2f;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public Transform DetectedTarget { get; private set; }

    private static RaycastHit2D[] _raycastBuffer = new RaycastHit2D[8];

    public bool HasLOS(Transform target)
    {
        if (target == null) return false;
        Vector2 origin = transform.position;
        Vector2 dir = ((Vector2)target.position - origin);
        float dist = dir.magnitude;
        if (dist <= 0.001f) return true;
        Vector2 norm = dir / dist;
        int hitCount = Physics2D.RaycastNonAlloc(origin, norm, _raycastBuffer, dist, obstacleMask);
        return hitCount == 0;
    }

    public bool CheckDetect(Transform player)
    {
        if (player == null) { DetectedTarget = null; return false; }
        float d = Vector2.Distance(transform.position, player.position);
        if (d <= detectionRadius && HasLOS(player)) { DetectedTarget = player; return true; }
        if (DetectedTarget == player && d > chaseRadius) DetectedTarget = null;
        return false;
    }

    public bool IsInAttackRange(Transform player)
    {
        if (player == null) return false;
        float d = Vector2.Distance(transform.position, player.position);
        return d <= attackRadius;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, chaseRadius);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
#endif
}
