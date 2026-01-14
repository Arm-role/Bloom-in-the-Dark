using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private InventoryController inventory;
    [SerializeField] private HotbarController hotbar;
    public void OnEnterInventory()
    {
        inventoryPanel.SetActive(true);
        hotbar.gameObject.SetActive(false);
        inventory.OpenInventory();
    }

    public void OnExitInventory()
    {
        inventoryPanel.SetActive(false);
        hotbar.gameObject.SetActive(true);
        inventory.CloseInventory();
    }
}