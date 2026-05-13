using UnityEngine;

[CreateAssetMenu(menuName = "Game/LootTableData")]
public class LootTableData : ScriptableObject
{
  [Header("EXP")]
  public ExpData expData;

  [Header("Drop")]
  public DropData[] drops;

  [System.Serializable]
  public class ExpData
  {
    public int minAmount = 1;
    public int maxAmount = 1;
    public float bonusChance; 
  }

  [System.Serializable]
  public class DropData
  {
    public ItemDefinition item;
    public int minAmount = 1;
    public int maxAmount = 1;
    public float bonusChance;
  }
}