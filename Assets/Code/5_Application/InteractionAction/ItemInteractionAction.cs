using System.Collections;
using UnityEngine;

public class ItemInteractionAction : MonoBehaviour
{
    //----ObjectInteraction----//
    private IItemInstance _itemInstance;

    private IDragDropController _dragDropController;
    private InteractionService _interactionService;
    private PlayerInventory _playerInventory;

    private ParticalService _particalService;

    private Vector2 lastPointerPosition;

    public ItemInteractionAction(
        IDragDropController dragDropController,
        InteractionService interactionService,
        PlayerInventory playerInventory,
        ParticalService particalService)
    {
        _dragDropController = dragDropController;
        _interactionService = interactionService;
        _playerInventory = playerInventory;

        _particalService = particalService;

        _dragDropController.OnRequestDisable += Dispose;
        _dragDropController.OnInteraction += ProcessInteractionContext;
    }

    private void Dispose()
    {
        if (_dragDropController != null)
        {
            _dragDropController.OnRequestDisable -= Dispose;
            _dragDropController.OnInteraction -= ProcessInteractionContext;
        }
    }
    private void ProcessInteractionContext(InteractionContext result)
    {
        if (result.ShouldClearItem) ClearItem();
        if (result.UseSourceItem) SetItem(GetItemOnSlot());
        if (result.LastPointerPosition != null) lastPointerPosition = result.LastPointerPosition;

        if (result.TargetCollider != null)
        {
            var targetcoll = result.TargetCollider;
            IDrop drop = _interactionService.GetDropResolve(_itemInstance.ItemData.Type, targetcoll);

            if (drop == null) return;
            var dropResult = drop.Execute(_itemInstance);

            ProcessDropResult(dropResult, _itemInstance, targetcoll);
        }

        if (result.IsPrimaryAction)
        {
            IPrimaryAction action = _interactionService.GetPrimaryActionResolve(_itemInstance.ItemData.Type, _itemInstance.ItemData.Name);

            if (action == null) return;
            var actionResult = action.Execute(_itemInstance);

            ProcessPrimaryResult(actionResult, _itemInstance);
        }
    }
    private async void ProcessPrimaryResult(PrimaryActionExecutionResult result, IItemInstance sourceItem)
    {
        if (result == null) return;

        if (result.SourceItemInstance != null)
        {
            result.SourceItemInstance.Invoke(sourceItem);
        }

        if (result.ParticleToPlay != null)
        {
            Debug.Log($"Player Particle {result.ParticleToPlay}");
            _particalService.Play(result.ParticleToPlay, lastPointerPosition);
        }

        if (await result.ShouldSpawnSelf)
        {
            Debug.Log($"SpawnObject {sourceItem.ItemData.Name}");
        }
    }
    private async void ProcessDropResult(DropExecutionResult result, IItemInstance sourceItem, Collider2D targetCollider)
    {
        if (result == null) return;

        if (result.TargetInteraction != null)
        {
            result.TargetInteraction.Invoke(targetCollider);
        }

        if (result.SourceItemInstance != null)
        {
            result.SourceItemInstance.Invoke(sourceItem);
        }

        if (result.ParticleToPlay != null)
        {
            Debug.Log($"Player Particle {result.ParticleToPlay}");
        }

        if (await result.ShouldDestroyTarget)
        {
            if (targetCollider.TryGetComponent<IInteractable>(out var targetObject))
            {
                targetObject.RequestDestruction();
            }
        }
    }

    private IItemInstance GetItemOnSlot()
    {
        var slot = _playerInventory.GetHotbarSlotSelected();
        if (slot.IsEmpty) return null;

        return slot.Item;
    }
    private void SetItem(IItemInstance itemInstance) => _itemInstance = itemInstance;
    private void ClearItem() => _itemInstance = null;
}
