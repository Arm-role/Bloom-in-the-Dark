using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new ToolItem", menuName = "Item/New ToolItem")]
public class ToolItem : Item, IToolItemData
{
    [SerializeField] private InputStrategyBinding[] strategyBindings;

    [Header("ToolData")]
    [SerializeField] private int level;
    [SerializeField] private float attackRange;
    [SerializeField] private int energyReduceEachAction;

    [Header("Identification")]
    [SerializeField] private int skillID;
    [SerializeField] private string skillName;

    [Header("Timing")]
    [SerializeField] private float cooldown;
    [SerializeField] private float cooldownPerLevel;

    [Header("Range & Area")]
    [SerializeField] private float range;
    [SerializeField] private float rangePerLevel;

    [Header("Damage")]
    [SerializeField] private float damage;
    [SerializeField] private float damagePerLevel;

    [Header("Crowd Control")]
    [SerializeField] private float knockForce;
    [SerializeField] private float knockDuration;
    [SerializeField] private float knockForcePerLevel;
    [SerializeField] private float knockDurationPerLevel;

    public int Level => level;
    public float AttackRange => attackRange;
    public float EnergyReduceEachAction => energyReduceEachAction;

    public override EItemType Type => EItemType.Tool;
    public override IReadOnlyList<InputStrategyBinding> StrategyBindings => strategyBindings;
    public override int MaxStackSize => 1;

    public bool HasBonus { get; set; }

    public int SkillID => skillID;
    public string SkillName => skillName;

    public float BaseCooldown => cooldown;
    public float CooldownPerLevel => cooldownPerLevel;

    public float BaseRange => range;
    public float RangePerLevel => rangePerLevel;

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
        EStatType.KnockForce => KnockForce,
        EStatType.KnockDuration => KnockDuration,
        _ => 0
    };

    public float GetPerLevelStat(EStatType stat) => stat switch
    {
        EStatType.Damage => DamagePerLevel,
        EStatType.Range => RangePerLevel,
        EStatType.KnockForce => KnockForcePerLevel,
        EStatType.KnockDuration => KnockDurationPerLevel,
        _ => 0
    };
}