using System.Threading.Tasks;

public interface ITileAction
{
    bool CanInteract(InteractionHandleContext ctx, TileBaseDataState state);
    Task<bool> TryInteract(InteractionHandleContext ctx, TileBaseDataState state);
}