using UnityEngine;

public interface IEnemySkill
{
  float MinRange { get; }
  float MaxRange { get; }
  int Priority { get; }
  float Weight { get; }
  float Cooldown { get; }
  bool IsReady { get; }
  bool IsExecuting { get; }

  void Initialize(Transform owner, EnemyCombat combat);
  void StartUse(Vector2 direction);
}

public interface ISkillFactory
{
  IEnemySkill Create(LayerMask targetMask);
}

public abstract class SkillDefinitionSO : ScriptableObject
{
  public abstract IEnemySkill Create(LayerMask targetMask);
}