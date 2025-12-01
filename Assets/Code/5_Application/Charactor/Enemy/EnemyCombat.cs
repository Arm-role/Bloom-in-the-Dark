using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    private List<IEnemySkill> _skills = new List<IEnemySkill>();
    public Transform Target;

    public Action<string> OnPlayAttack; 
    public Action OnPlayHit;
    public Action OnPlayDash;
    public Action OnPlaySlam;

    public Action<float> OnRequestStopMovement;

    public Action<Vector2, float> OnRequestDash;

    public void Initialize(Transform target)
    {
        Target = target;
    }

    public void AddSkill(IEnemySkill skill)
    {
        _skills.Add(skill);
        skill.Initialize(transform, this);
    }

    public IEnemySkill GetBestSkill(float distance)
    {
        foreach (var s in _skills)
        {
            if (s.IsReady && distance <= s.Range)
                return s;
        }
        return null;
    }

    public void UseSkill(IEnemySkill skill, Vector2 dir)
    {
        if (skill == null) return;
        skill.StartUse(dir);
    }

    public bool AnySkillReadyInRange(float distance) => GetBestSkill(distance) != null;
}