using System;
using System.Threading.Tasks;
using UnityEngine;

public class TileInteractableAdapter : IWorldInteractable
{
    private readonly TileBaseDataState _state;
    private readonly float _priority;

    public TileInteractableAdapter(TileBaseDataState state, float priority = 0.5f)
    {
        _state = state;
        _priority = priority;
    }

    public float InteractionPriority => _priority;
    public ETileBlockType Type => _state.WorldInteractableType;

    public event Action<GameObject> OnRequestDestruction;

    public virtual bool CanInteract(InteractionHandleContext context)
    {
        if (_state.HasPlacedObject) return false;
        return true;
    }

    public virtual Task<bool> TryInteract(InteractionHandleContext context)
    {
        return Task.FromResult(false);
    }
}
