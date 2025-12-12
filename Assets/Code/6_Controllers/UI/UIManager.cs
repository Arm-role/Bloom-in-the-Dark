using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private InventoryController inventoryController;

    private bool isOpen;

    private IPlayerInput _playerInput;
    public void Initialze(IPlayerInput playerInput)
    {
        _playerInput = playerInput;
        _playerInput.OnInventoryToggle += ToggleInventory;
    }
    private void OnDisable()
    {
        _playerInput.OnInventoryToggle -= ToggleInventory;
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            inventoryController.OpenInventory();
            inventoryController.RefreshAllSlots();
        }
        else
        {
            inventoryController.CloseInventory();
        }
    }
}