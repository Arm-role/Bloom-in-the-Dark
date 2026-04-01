using UnityEngine;

public class SelfTargetStrategy : ITargetStrategy
{
    private readonly WorldTileManager _world;

    public SelfTargetStrategy(WorldTileManager world) => _world = world;

    public TargetResult Resolve(InteractionHandleContext ctx, ITargetingConfig config)
    {
        if (!ctx.PlayerPosition.HasValue)
            return TargetResult.Invalid;

        var cell = _world.GetCellFromWorld(ctx.PlayerPosition.Value);
        if (cell == null)
            return TargetResult.Invalid;

        return new TargetResult(
            new[] { cell },
            ctx.PlayerPosition.Value,
            Vector2.zero,
            Vector2.zero);
    }
}