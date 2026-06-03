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

    [Header("Optional cap")]
    [Tooltip("จำกัดจำนวน item นี้ที่ player ถือ (Inventory + Altar) — drop เฉพาะตอน count < cap, clamp ให้พอดี cap. 0 = ไม่ filter")]
    public int globalCap = 0;
  }
}