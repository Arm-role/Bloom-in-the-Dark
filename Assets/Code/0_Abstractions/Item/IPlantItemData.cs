using UnityEngine;

public interface IPlantItemData
{
    int SkillID { get; }
    string SkillName { get; }
    float BaseCooldown { get; }
    float BaseCastTime { get; }       
    float BaseDuration { get; }       
    float BaseRange { get; }          
    float BaseAreaRadius { get; }     
    float BaseDamage { get; }       
    float BaseLifetime { get; }

    float AreaRadiusPerLevel { get; }
    float DamagePerLevel { get; }
    float LifetimePerLevel { get; }
    float DurationPerLevel { get; }

}
