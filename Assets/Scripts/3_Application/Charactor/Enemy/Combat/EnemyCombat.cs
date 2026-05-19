using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
  public IReadOnlyList<IEnemySkill> GetSkills() => _skills.AsReadOnly();
  private List<IEnemySkill> _skills = new List<IEnemySkill>();

  private EnemyController _owner;
  public Transform Target => _owner.CurrentTarget;

  public Action<string> OnPlayAttack;
  public Action OnPlayHit;
  public Action OnAnimationImpact;

  public Action<Transform> OnTargetDeath;

  public Action<float> OnRequestStopMovement;

  public Action<Vector2, float> OnRequestDash;
  public Action<Vector2> OnPlayPrepareDash;
  public Action<Vector2> OnPlayDash;
  public Action OnPlayEndDash;

  public Action<Vector2, float> OnRequestSlam;
  public Action OnPlaySlamWindup;
  public Action OnPlaySlamRise;
  public Action OnPlaySlamFall;
  public Action OnPlaySlamLand;
  public Action OnPlaySlamRecovery;

  public Action OnPlaySpearSlamWindup;
  public Action OnPlaySpearSlamRise;
  public Action OnPlaySpearSlamFall;
  public Action OnPlaySpearSlamLand;
  public Action OnPlaySpearSlamRecovery;

  public Action OnPlaySpearDiveWindup;
  public Action OnPlaySpearDiveRise;
  public Action OnPlaySpearDivePeak;
  public Action OnPlaySpearDiveFall;
  public Action OnPlaySpearDiveLand;
  public Action OnPlaySpearDiveRecovery;

  public Action OnRequestDisableCollision;
  public Action OnRequestEnableCollision;

  public Action<bool> OnRequestHoldPosition;
  public Action<bool> OnNavigationPauseRequested;

  public Action OnRequestDisablePhysics;
  public Action OnRequestEnablePhysics;
  public void Initialize(EnemyController controller)
  {
    _owner = controller;
  }

  public void AddSkill(IEnemySkill skill)
  {
    _skills.Add(skill);
    skill.Initialize(transform, this);
  }

  public IEnemySkill SelectSkill(float distance)
  {
    // ✅ clamp ไม่ให้ติดลบ — overlap = อยู่ที่ระยะ 0
    distance = Mathf.Max(distance, 0f);

    IEnemySkill best = null;
    float bestScore = float.MinValue;

    foreach (var s in _skills)
    {
      if (!s.IsReady) continue;
      if (distance < s.MinRange) continue;
      if (distance > s.MaxRange) continue;

      float score = s.Priority + UnityEngine.Random.value * s.Weight;
      if (score > bestScore)
      {
        bestScore = score;
        best = s;
      }
    }

    return best;
  }

  public void UseSkill(IEnemySkill skill, Vector2 dir)
  {
    if (skill == null) return;
    skill.StartUse(dir);
  }

  public float GetMinAttackRange()
  {
    float r = float.MaxValue;
    foreach (var s in _skills)
      r = Mathf.Min(r, s.MinRange);
    return r;
  }

  public float GetMaxAttackRange()
  {
    float r = 0f;
    foreach (var s in _skills)
      r = Mathf.Max(r, s.MaxRange);
    return r;
  }

  public T GetSkill<T>() where T : class, IEnemySkill
  {
    foreach (var s in _skills)
      if (s is T typed)
        return typed;
    return null;
  }

  public bool AnySkillReadyInRange(float dist)
  {
    dist = Mathf.Max(dist, 0f); // ✅

    foreach (var s in _skills)
    {
      if (!s.IsReady) continue;
      if (dist >= s.MinRange && dist <= s.MaxRange)
        return true;
    }
    return false;
  }
}
