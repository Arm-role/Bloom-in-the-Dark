
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private PlayerInventory _playerInventory;

    private IInventoryView _hotbarView;

    private HotbarController _hotbarController;

    public void Initialize(IInventoryView hotbarView, HotbarController hotbarController, PlayerInventory playerInventory)
    {
        _playerInventory = playerInventory;
        _hotbarController = hotbarController;

        _hotbarView = hotbarView;

        _hotbarController.SetTotalSlots(_playerInventory.Hotbar.Capacity);
        _hotbarView.CreateSlots(_playerInventory.Hotbar.Capacity);

        _hotbarView.OnSlotClicked += _playerInventory.HotbarState.SelectSlot;

        foreach (var slot in _playerInventory.Hotbar.Slots)
        {
            slot.OnSlotChanged += HandleSlotChange;
        }

        _hotbarController.SlotSelected += _playerInventory.HotbarState.SelectSlot;
    }

    private void OnDisable()
    {
        _hotbarView.OnSlotClicked -= _playerInventory.HotbarState.SelectSlot;
        _hotbarController.SlotSelected -= _playerInventory.HotbarState.SelectSlot;

    }
    public void MockInstall(IItemData testItemA, IItemData testItemB)
    {
        if (testItemA != null)
            _playerInventory.Hotbar.TryAddItem(new PlantItemInstance(testItemA, 1f, 3), 5);
        if (testItemB != null)
            _playerInventory.Hotbar.TryAddItem(new PlantItemInstance(testItemB, 1f, 3), 2);
    }


    private void OnDestroy()
    {
        if (_playerInventory != null)
        {
            _hotbarView.OnSlotClicked -= _playerInventory.HotbarState.SelectSlot;
            foreach (var slot in _playerInventory.Hotbar.Slots)
            {
                slot.OnSlotChanged -= HandleSlotChange;
            }
        }
    }


    private void HandleSlotChange(InventorySlot slotModel)
    {
        int index = _playerInventory.Hotbar.Slots.IndexOf(slotModel);

        SlotDisplayData displayData = new SlotDisplayData();
        if (!slotModel.IsEmpty)
        {
            displayData.Icon = slotModel.Item.ItemData.Icon;
            displayData.Amount = slotModel.Amount;
        }

        _hotbarView.UpdateSlot(index, displayData);
    }

    private void OnMainInventorySlotClicked(int slotIndex)
    {
        Debug.Log($"Main inventory slot {slotIndex} was clicked!");

        int firstEmptyHotbarSlot = _playerInventory.Hotbar.Slots.FindIndex(s => s.IsEmpty);
        if (firstEmptyHotbarSlot != -1)
        {
            _playerInventory.MoveFromInventoryToHotbar(slotIndex, firstEmptyHotbarSlot);
        }
    }

    private void RefreshAllSlots()
    {
        var allData = new List<SlotDisplayData>();
        foreach (var slotModel in _playerInventory.Hotbar.Slots)
        {
            var displayData = new SlotDisplayData();
            if (!slotModel.IsEmpty)
            {
                displayData.Icon = slotModel.Item.ItemData.Icon;
                displayData.Amount = slotModel.Amount;
            }
            allData.Add(displayData);
        }
        _hotbarView.UpdateAllSlots(allData);
    }
}