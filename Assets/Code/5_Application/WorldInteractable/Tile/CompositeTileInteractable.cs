using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

public class CompositeTileInteractable : IWorldInteractable
{
    public float InteractionPriority { get; private set; }
    public ETileBlockType Type { get; private set; }    

    private readonly List<ITileAction> _actions;
    private readonly TileBaseDataState _state;

    public CompositeTileInteractable(TileBaseDataState state, IEnumerable<ITileAction> actions)
    {
        _state = state;
        _actions = actions.ToList();
    }

    public bool CanInteract(InteractionHandleContext ctx)
        => _actions.Any(a => a.CanInteract(ctx, _state));

    public Task<bool> TryInteract(InteractionHandleContext ctx)
    {
        foreach (var action in _actions)
        {
            if (action.CanInteract(ctx, _state))
                return action.TryInteract(ctx, _state);
        }

        return Task.FromResult(false);
    }
}
