using UnityEngine;

public class EnemySensor : MonoBehaviour
{
    public float detectionRadius = 6f;
    public float chaseRadius = 10f;
    public float attackRadius = 1.2f;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public Transform DetectedTarget { get; private set; }

    public bool HasLOS(Transform target)
    {
        Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
        float dist = Vector2.Distance(transform.position, target.position);
        if (Physics2D.Raycast(transform.position, dir, dist, obstacleMask))
            return false;
        return true;
    }

    public bool CheckDetect(Transform player)
    {
        float d = Vector2.Distance(transform.position, player.position);
        if (d <= detectionRadius && HasLOS(player))
        {
            DetectedTarget = player;
            return true;
        }
        return false;
    }

    public bool IsInAttackRange(Transform player)
    {
        float d = Vector2.Distance(transform.position, player.position);
        return d <= attackRadius;
    }
}