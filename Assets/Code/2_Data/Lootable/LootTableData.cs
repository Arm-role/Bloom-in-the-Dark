using UnityEngine;

[CreateAssetMenu(menuName = "Game/LootTableData")]
public class LootTableData : ScriptableObject
{
    public DropData[] drops;
    
    [System.Serializable]
    public class DropData
    {
        public Item item;
        public int minAmount = 1;
        public int maxAmount = 1;
        public float bonusChance; // โอกาสได้เพิ่ม
    }
}
