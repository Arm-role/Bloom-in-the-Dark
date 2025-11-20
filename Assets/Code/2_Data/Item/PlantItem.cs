using UnityEngine;

[CreateAssetMenu(fileName = "new PlantItem", menuName = "Item/New PlantItem")]
public class PlantItem : Item, IPlantItemData
{
    [SerializeField] private EItemStategyType stategyType;

    [Header("SkillData")]
    [SerializeField] private int skillID;
    [SerializeField] private string skillName;
    [SerializeField] private float lifetime;
    [SerializeField] private float cooldown;
    [SerializeField] private float castTime;
    [SerializeField] private float duration;
    [SerializeField] private float range;
    [SerializeField] private float areaRadius;
    [SerializeField] private float damage;
    [SerializeField] private float areaRadiusPerLevel;
    [SerializeField] private float damagePerLevel;
    [SerializeField] private float lifetimePerLevel;
    [SerializeField] private float durationPerLevel;

    public override EItemType Type => EItemType.Plant;
    public override EItemStategyType StategyType => stategyType;
    public override int MaxStackSize => 64;

    public int SkillID => skillID;
    public string SkillName => skillName;
    public float BaseLifetime => lifetime;
    public float BaseCooldown => cooldown;
    public float BaseCastTime => castTime;
    public float BaseDuration => duration;
    public float BaseRange => range;
    public float BaseAreaRadius => areaRadius;
    public float BaseDamage => damage;

    public float AreaRadiusPerLevel => areaRadiusPerLevel;

    public float DamagePerLevel => damagePerLevel;

    public float LifetimePerLevel => lifetimePerLevel;

    public float DurationPerLevel => durationPerLevel;
}
