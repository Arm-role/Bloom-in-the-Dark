using UnityEngine;

public interface IEnemySkill
{
    float MinRange { get; }
    float MaxRange { get; }
    int Priority { get; }    
    float Weight { get; }     

    float Cooldown { get; }
    bool IsReady { get; }

    void Initialize(Transform owner, EnemyCombat combat);
    void StartUse(Vector2 direction);
}