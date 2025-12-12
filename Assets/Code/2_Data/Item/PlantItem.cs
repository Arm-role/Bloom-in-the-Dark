using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new PlantItem", menuName = "Item/New PlantItem")]
public class PlantItem : Item, IPlantItemData
{
    [SerializeField] private InputStrategyBinding[] strategyBindings;

    [Header("Identification")]
    [SerializeField] private int skillID;
    [SerializeField] private string skillName;

    [Header("Timing")]
    [SerializeField] private float castTime;
    [SerializeField] private float cooldown;
    [SerializeField] private float lifetime;
    [SerializeField] private float duration;
    [SerializeField] private float castTimePerLevel;
    [SerializeField] private float cooldownPerLevel;
    [SerializeField] private float lifetimePerLevel;
    [SerializeField] private float durationPerLevel;

    [Header("Range & Area")]
    [SerializeField] private float range;
    [SerializeField] private float areaRadius;
    [SerializeField] private float rangePerLevel;
    [SerializeField] private float areaRadiusPerLevel;

    [Header("Damage")]
    [SerializeField] private float damage;
    [SerializeField] private float damagePerLevel;

    [Header("Crowd Control")]
    [SerializeField] private float knockForce;
    [SerializeField] private float knockDuration;
    [SerializeField] private float knockForcePerLevel;
    [SerializeField] private float knockDurationPerLevel;

    public override EItemType Type => EItemType.Plant;
    public override IReadOnlyList<InputStrategyBinding> StrategyBindings => strategyBindings;
    public override int MaxStackSize => 64;

    public int SkillID => skillID;
    public string SkillName => skillName;

    public float BaseCastTime => castTime;
    public float BaseCooldown => cooldown;
    public float BaseLifetime => lifetime;
    public float BaseDuration => duration;
    public float CastTimePerLevel => castTimePerLevel;
    public float CooldownPerLevel => cooldownPerLevel;
    public float LifetimePerLevel => lifetimePerLevel;
    public float DurationPerLevel => durationPerLevel;

    public float BaseRange => range;
    public float BaseAreaRadius => areaRadius;
    public float RangePerLevel => rangePerLevel;
    public float AreaRadiusPerLevel => areaRadiusPerLevel;

    public float BaseDamage => damage;
    public float DamagePerLevel => damagePerLevel;

    public float KnockForce => knockForce;
    public float KnockDuration => knockDuration;
    public float KnockForcePerLevel => knockForcePerLevel;
    public float KnockDurationPerLevel => knockDurationPerLevel;

    public float GetBaseStat(EStatType stat) => stat switch
    {
        EStatType.Damage => BaseDamage,
        EStatType.Range => BaseRange,
        EStatType.AreaRadius => BaseAreaRadius,
        EStatType.Lifetime => BaseLifetime,
        EStatType.KnockForce => KnockForce,
        EStatType.KnockDuration => KnockDuration,
        _ => 0
    };

    public float GetPerLevelStat(EStatType stat) => stat switch
    {
        EStatType.Damage => DamagePerLevel,
        EStatType.Range => RangePerLevel,
        EStatType.AreaRadius => AreaRadiusPerLevel,
        EStatType.Lifetime => LifetimePerLevel,
        EStatType.KnockForce => KnockForcePerLevel,
        EStatType.KnockDuration => KnockDurationPerLevel,
        _ => 0
    };
}
