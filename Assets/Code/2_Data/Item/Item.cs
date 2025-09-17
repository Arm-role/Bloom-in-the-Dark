using System.Collections;
using System.Runtime.ExceptionServices;
using UnityEngine;

[CreateAssetMenu(fileName ="new Item", menuName ="Item/New Item")]
public class Item : ScriptableObject, IItemData
{
    [Header("ItemData")]
    [SerializeField] private int itemId;
    [SerializeField] private string itemName;
    [SerializeField] private EItemType itemType;
    [SerializeField] private int maxStackSize;
    [SerializeField] private Sprite itemIcon;

    public int ID => itemId;
    public string Name => itemName;
    public EItemType Type => itemType;
    public Sprite Icon => itemIcon;

    public int MaxStackSize => maxStackSize;
}
