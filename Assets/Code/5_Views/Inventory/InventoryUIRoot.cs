using UnityEngine;

public class InventoryUIRoot : MonoBehaviour, IInventoryUIRoot
{
  [SerializeField] private GameObject hotbarPanel;
  [SerializeField] private GameObject inventoryPanel;
  [SerializeField] private GameObject energybarPanel;
  [SerializeField] private Transform hotbarRoot;
  [SerializeField] private Transform inventoryRoot;

  private HotbarController _hotbar;

  public bool IsOpen { get; private set; }

  public void Initialzed(HotbarController hotbar)
  {
    _hotbar = hotbar;
  }

  public void Open()
  {
    IsOpen = true;

    hotbarPanel.transform.SetParent(inventoryRoot);
    inventoryPanel.SetActive(true);
    energybarPanel.SetActive(false);
    _hotbar.enabled = false;
  }

  public void Close()
  {
    IsOpen = false;

    hotbarPanel.transform.SetParent(hotbarRoot);
    inventoryPanel.SetActive(false);
    energybarPanel.SetActive(true);
    _hotbar.enabled = true;
  }
}