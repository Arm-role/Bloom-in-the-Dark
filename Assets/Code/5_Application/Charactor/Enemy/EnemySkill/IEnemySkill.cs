using UnityEngine;

public interface IEnemySkill
{
    float Range { get; }
    float Cooldown { get; }
    bool IsReady { get; }
    void Initialize(Transform owner, EnemyCombat combat);
    void StartUse(Vector2 direction);
}