using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
  public IReadOnlyList<IEnemySkill> GetSkills() => _skills.AsReadOnly();
  private List<IEnemySkill> _skills = new List<IEnemySkill>();

  public Transform Target;

  public Action<string> OnPlayAttack;
  public Action OnPlayHit;
  public Action OnPlayDash;
  public Action OnPlaySlam;

  public Action<float> OnRequestStopMovement;
  public Action<Vector2, float> OnRequestDash;
  public Action<bool> OnRequestHoldPosition;
  public void Initialize(Transform target) => Target = target;

  public void AddSkill(IEnemySkill skill)
  {
    _skills.Add(skill);
    skill.Initialize(transform, this);
  }

  public IEnemySkill SelectSkill(float distance)
  {
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

        //Debug.Log(s.ToString());
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

  public bool AnySkillReadyInRange(float distance)
  {
    foreach (var s in _skills)
    {
      if (!s.IsReady) continue;
      if (distance >= s.MinRange && distance <= s.MaxRange)
        return true;
    }
    return false;
  }
}
