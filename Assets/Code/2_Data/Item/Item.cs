using UnityEngine;

public class Item : ScriptableObject, IItemData
{
    [Header("ItemData")]
    [SerializeField] private int itemId;
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemIcon;

    public int ID => itemId;
    public string Name => itemName;
    public Sprite Icon => itemIcon;

    public virtual EItemType Type { get; set; }
    public virtual EItemStategyType StategyType { get; set; }
    public virtual int MaxStackSize { get; set; }


    private void OnValidate()
    {
        itemName = name;
    }
}
