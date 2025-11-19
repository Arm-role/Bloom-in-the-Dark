using UnityEngine;

public interface IPlantItemData
{
    int SkillID { get; }
    string SkillName { get; }
    float Cooldown { get; }
    float CastTime { get; }       
    float Duration { get; }       
    float Range { get; }          
    float AreaRadius { get; }     
    float Damage { get; }       
    float Lifetime { get; }
}
