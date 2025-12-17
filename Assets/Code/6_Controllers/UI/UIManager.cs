using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private InventoryController inventory;

    public void OnEnterInventory()
    {
        inventoryPanel.SetActive(true);
        inventory.OpenInventory();
    }

    public void OnExitInventory()
    {
        inventoryPanel.SetActive(false);
        inventory.CloseInventory();
    }
}