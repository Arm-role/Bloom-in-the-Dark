using UnityEngine;

public interface IToolItemData : IEnergyReduce
{
    float GetBaseStat(EStatType stat);
    float GetPerLevelStat(EStatType stat);

    public int Level { get; }
    public float AttackRange { get; }
    int SkillID { get; }
    string SkillName { get; }

    float BaseCooldown { get; }
    float CooldownPerLevel { get; }

    float BaseRange { get; }
    float RangePerLevel { get; }

    float BaseDamage { get; }
    float DamagePerLevel { get; }

    float KnockForce { get; }
    float KnockDuration { get; }
    float KnockForcePerLevel { get; }
    float KnockDurationPerLevel { get; }
}
