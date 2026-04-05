using System.Collections.Generic;
using UnityEngine;

public class EnemySensor : MonoBehaviour
{
  [SerializeField] private SensorProfileSO profile;
  
  public LayerMask targetMask { get; set; }
  public LayerMask obstacleMask { get; set; }

  private float detectionRadius;
  public float chaseRadius;

  private Collider2D[] _overlapBuffer = new Collider2D[32];
  private static RaycastHit2D[] _raycastBuffer = new RaycastHit2D[8];

  private readonly List<Transform> _visibleTargets =
      new List<Transform>();

  public IReadOnlyList<Transform> VisibleTargets =>
      _visibleTargets;

  public void AutoSetup(EnemyCombat combat)
  {
    float max = combat.GetMaxAttackRange();

    detectionRadius = max * 1.5f;
    chaseRadius = max * 2.5f;

    Debug.Log(
      $"[Sensor AutoSetup] detection={detectionRadius}, chase={chaseRadius}"
    );
  }

  // =============================
  // MAIN SENSOR SCAN
  // =============================

  public void ScanTargets()
  {
    _visibleTargets.Clear();

    int count = Physics2D.OverlapCircleNonAlloc(
        transform.position,
        detectionRadius,
        _overlapBuffer,
        targetMask
    );

    for (int i = 0; i < count; i++)
    {
      Transform t = _overlapBuffer[i].transform;

      if (t == null)
        continue;

      if (!HasLOS(t))
        continue;

      _visibleTargets.Add(t);
    }
  }

  // =============================
  // LINE OF SIGHT
  // =============================

  public bool HasLOS(Transform target)
  {
    if (target == null)
      return false;

    Vector2 origin = transform.position;

    Vector2 dir =
        (Vector2)target.position - origin;

    float dist = dir.magnitude;

    if (dist <= 0.001f)
      return true;

    Vector2 norm = dir / dist;

    int hitCount =
        Physics2D.RaycastNonAlloc(
            origin,
            norm,
            _raycastBuffer,
            dist,
            obstacleMask
        );

    return hitCount == 0;
  }

  // =============================
  // ATTACK RANGE CHECK
  // =============================

  public bool CanAttack(
      Transform target,
      EnemyCombat combat)
  {
    if (target == null)
      return false;

    float dist =
        Vector2.Distance(
            transform.position,
            target.position
        );

    foreach (var s in combat.GetSkills())
    {
      if (s.IsReady &&
          dist >= s.MinRange &&
          dist <= s.MaxRange)
      {
        return true;
      }
    }

    return false;
  }

  public bool IsInAttackRange(
      Transform target,
      EnemyCombat combat)
  {
    if (target == null)
      return false;

    float dist =
        Vector2.Distance(
            transform.position,
            target.position
        );

    return dist <= combat.GetMaxAttackRange();
  }

#if UNITY_EDITOR
  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.cyan;

    Gizmos.DrawWireSphere(
        transform.position,
        detectionRadius
    );

    Gizmos.color = Color.yellow;

    Gizmos.DrawWireSphere(
        transform.position,
        chaseRadius
    );

    // draw visible targets
    if (Application.isPlaying)
    {
      Gizmos.color = Color.green;

      foreach (var t in _visibleTargets)
      {
        if (t == null) continue;

        Gizmos.DrawLine(
            transform.position,
            t.position
        );
      }
    }
  }
#endif

}
