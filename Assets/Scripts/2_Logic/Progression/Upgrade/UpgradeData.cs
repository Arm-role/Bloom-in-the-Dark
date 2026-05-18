using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Roguelite/Upgrade")]
public class UpgradeData : ScriptableObject
{
  [SerializeField] private GameKeyAsset gameKey;
  [SerializeField] private string upgradeName;
  [SerializeField] private string description;

  public string UpgradeName => upgradeName;
  public string Description => description;
  public GameTag Gamekey => gameKey.RuntimeTag;
  public StatModifier[] modifiers;
}