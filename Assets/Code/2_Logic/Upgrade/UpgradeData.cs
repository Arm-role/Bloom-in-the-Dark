using UnityEngine;

[CreateAssetMenu(menuName = "Roguelite/Upgrade")]
public class UpgradeData : ScriptableObject
{
  public string upgradeName;
  public string description;

  public ItemKey _itemKey;

  public GameTag itemKey => _itemKey.RuntimeTag;
  public StatModifier[] modifiers;
}
