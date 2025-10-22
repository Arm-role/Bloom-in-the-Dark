using UnityEngine;

[CreateAssetMenu(fileName = "new PlantItem", menuName = "Item/New PlantItem")]
public class PlantItem : Item, IPlantItemData
{
    [SerializeField] private EItemStategyType stategyType;

    [Header("PlantData")]
    [SerializeField] private int skillID;
    [SerializeField] private string skillName;
    [SerializeField] private float weight;
    [SerializeField] private float lifetime;

    public int SkillID => skillID;
    public string SkillName => skillName;
    public float Weight => weight;
    public float Lifetime => lifetime;

    public override EItemType Type => EItemType.Plant;
    public override EItemStategyType StategyType => stategyType;
    public override int MaxStackSize => 128;
}
