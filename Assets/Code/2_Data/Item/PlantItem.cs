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

    public override EItemType Type => EItemType.Plant;
    public override EItemStategyType StategyType => stategyType;
    public override int MaxStackSize => 128;

    public int SkillID => skillID;
    public string SkillName => skillName;
    public float Lifetime => lifetime;
    public float Cooldown => cooldown;
    public float CastTime => castTime;
    public float Duration => duration;
    public float Range => range;
    public float AreaRadius => areaRadius;
    public float Damage => damage;
}
