using UnityEngine;

public interface IPlantItemData
{
    float GetBaseStat(EStatType stat);
    float GetPerLevelStat(EStatType stat);

    int SkillID { get; }
    string SkillName { get; }

    float BaseCastTime { get; }       
    float BaseCooldown { get; }
    float BaseLifetime { get; }
    float BaseDuration { get; }
    float CastTimePerLevel { get; }
    float CooldownPerLevel { get; }
    float LifetimePerLevel { get; }
    float DurationPerLevel { get; }

    float BaseRange { get; }          
    float BaseAreaRadius { get; }
    float RangePerLevel { get; }
    float AreaRadiusPerLevel { get; }

    float BaseDamage { get; }
    float DamagePerLevel { get; }

    float KnockForce { get; }
    float KnockDuration { get; }
    float KnockForcePerLevel { get; }
    float KnockDurationPerLevel { get; }
}
