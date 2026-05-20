using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Roguelite/Upgrade")]
public class UpgradeData : ScriptableObject
{
  [SerializeField] private GameKeyAsset gameKey;
  [SerializeField] private string upgradeName;
  [TextArea]
  [SerializeField] private string description;
  [SerializeField] private Sprite cardSprite;

  public string UpgradeName => upgradeName;
  public string Description => description;
  public Sprite CardSprite => cardSprite;
  public GameTag Gamekey => gameKey.RuntimeTag;
  public StatModifier[] modifiers;
}