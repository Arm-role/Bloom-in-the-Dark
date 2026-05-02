using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetSelector
{
  private EnemyController owner;
  private TargetSelectorProfileSO selectorProfileSO;

  private Transform currentTarget;

  private readonly Dictionary<Transform, float> threatTable =
      new Dictionary<Transform, float>();

  public Transform CurrentTarget => currentTarget;

  private float threatDecayRate = 0.5f;

  public EnemyTargetSelector(
    EnemyController owner,
    TargetSelectorProfileSO selectorProfileSO)
  {
    this.owner = owner;
    this.selectorProfileSO = selectorProfileSO;
  }

  // =============================
  // REGISTER THREAT
  // =============================

  public void RegisterThreat(
     Transform target,
     float threat,
     bool accumulate = false)
  {
    if (target == null) return;

    if (target == owner.DefaultTarget)
    {
      if (!threatTable.ContainsKey(target))
        threatTable[target] = threat;
      return; 
    }

    if (!threatTable.ContainsKey(target))
    {
      threatTable[target] = threat;
      return;
    }

    if (accumulate)
      threatTable[target] += threat;
  }

  // =============================
  // REMOVE TARGET
  // =============================

  public void RemoveTarget(Transform target)
  {
    if (target == null)
      return;

    // ❗ DefaultTarget ห้ามโดน remove
    if (target == owner.DefaultTarget)
      return;

    threatTable.Remove(target);
    Debug.LogWarning($"Remove {target.name}");
    if (currentTarget == target)
      currentTarget = null;
  }

  // =============================
  // SELECT TARGET (MAIN LOGIC)
  // =============================

  public void SetThreat(Transform target, float threat)
  {
    if (target == null) return;
    if (target == owner.DefaultTarget) return;
    threatTable[target] = threat;
  }

  public void TickSelectTarget()
  {
    DecayThreat();

    // ----------------------------------
    // Remove current target if too far
    // ----------------------------------

    if (currentTarget != null &&
        currentTarget != owner.DefaultTarget)
    {
      float dist =
        Vector2.Distance(
          owner.transform.position,
          currentTarget.position
        );

      if (dist > owner.Sensor.chaseRadius)
      {
        RemoveTarget(currentTarget);
        currentTarget = null;
      }
    }

    // ----------------------------------
    // If no threat targets → fallback
    // ----------------------------------

    if (threatTable.Count == 0)
    {
      currentTarget = owner.DefaultTarget;
      return;
    }

    // ----------------------------------
    // Score evaluation
    // ----------------------------------

    float bestScore = float.MinValue;
    Transform bestTarget = null;
    bool hasHighPriorityTarget = false;

    foreach (var kv in threatTable)
    {
      Transform t = kv.Key;
      if (t == null) continue;

      float threatScore = kv.Value;
      float dist = Vector2.Distance(owner.transform.position, t.position);
      float distanceScore = 1f / Mathf.Max(dist, 0.5f);

      float score =
          threatScore * selectorProfileSO.ThreatWeight +
          distanceScore * selectorProfileSO.DistanceWeight;

      // Crowding penalty
      if (CrowdingTracker.Instance != null)
      {
        int attackers = Mathf.Min(
            CrowdingTracker.Instance.GetAttackerCount(t),
            selectorProfileSO.MaxCrowdingCount);
        if (t == currentTarget)
          attackers = Mathf.Max(0, attackers - 1);
        score -= attackers * selectorProfileSO.CrowdingPenaltyPerAttacker;
      }

      // ✅ track ว่ามี non-default target ที่ score ดีไหม
      if (t != owner.DefaultTarget && score > 0)
        hasHighPriorityTarget = true;

      if (score > bestScore)
      {
        bestScore = score;
        bestTarget = t;
      }
    }

    if (!hasHighPriorityTarget)
      bestTarget = owner.DefaultTarget;

    // ----------------------------------
    // Final fallback safety
    // ----------------------------------

    if (bestTarget == null)
    {
      bestTarget = owner.DefaultTarget;
      Debug.LogWarning($"bestTarget Target: null");
    }

    currentTarget = bestTarget;
  }

  // =============================
  // THREAT DECAY
  // =============================

  private void DecayThreat()
  {
    var keys = new List<Transform>(threatTable.Keys);

    foreach (var k in keys)
    {
      if (k == null) { threatTable.Remove(k); continue; }
      if (k == owner.DefaultTarget) continue; 

      threatTable[k] -= Time.deltaTime * selectorProfileSO.ThreatDecayRate;

      if (threatTable[k] <= selectorProfileSO.MinAggroThreshold)
        threatTable.Remove(k);
    }
  }
}
