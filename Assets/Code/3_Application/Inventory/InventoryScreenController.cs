
using UnityEngine;

public sealed class InventoryScreenController
{
  private readonly IInventoryUIRoot _uiRoot;
  private readonly InventoryController _inventoryController;
  public InventoryScreenController(
    IInventoryUIRoot uiRoot,
    InventoryController inventoryController)
  {
    _uiRoot = uiRoot;
    _inventoryController = inventoryController;
  }

  public void Open()
  {
    Debug.Log("Open");
    _uiRoot.Open();
    _inventoryController.OnInventoryOpened();
  }

  public void Close()
  {
    Debug.Log("close");
    _uiRoot.Close();
    _inventoryController.OnInventoryClosed();
  }
}
