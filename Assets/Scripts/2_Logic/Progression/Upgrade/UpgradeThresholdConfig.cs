using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Roguelite/UpgradeThreshold")]
public class UpgradeThresholdConfig : ScriptableObject
{
  [SerializeField] private GameKeyAsset key;
  public GameTag tag => key.RuntimeTag;

  public List<ThresholdBonus> thresholds;
}

[Serializable]
public class ThresholdBonus
{
  public int count;
  public UpgradeData reward;
  public bool altarUpgrade;
}