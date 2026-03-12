using UnityEngine;

public class DirectInteractStrategy : ITargetStrategy
{
    private readonly WorldTileManager _world;

    public DirectInteractStrategy(
        WorldTileManager world) => _world = world;

    public TargetResult Resolve(InteractionHandleContext ctx,ITargetingConfig config)
    {
        if (!ctx.PlayerPosition.HasValue || !ctx.PointerPosition.HasValue)
            return TargetResult.Invalid;

        Vector2 player = ctx.PlayerPosition.Value;
        Vector2 pointer = ctx.PointerPosition.Value;
        
        var cfg = (DirectInteractConfig)config;

        if (Vector2.Distance(player, pointer) > cfg.MaxDistance)
            return TargetResult.Invalid;

        var cell = _world.GetCellFromWorld(pointer);
        if (cell == null)
            return TargetResult.Invalid;

        return new TargetResult(
            new[] { cell },
            player,
            (pointer - player).normalized,
            pointer);
    }
}