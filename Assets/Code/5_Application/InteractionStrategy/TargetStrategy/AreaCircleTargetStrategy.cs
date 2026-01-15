using System.Collections.Generic;
using UnityEngine;

public class AreaCircleTargetStrategy : ITargetStrategy
{
    private readonly AreaCircleShape _shape;
    private readonly WorldTileManager _world;

    public AreaCircleTargetStrategy(
        AreaCircleShape shape,
        WorldTileManager world)
    {
        _shape = shape;
        _world = world;
    }

    public TargetResult Resolve(InteractionHandleContext ctx,ITargetingConfig config)
    {
        if (!ctx.PlayerPosition.HasValue || !ctx.PointerPosition.HasValue)
            return TargetResult.Invalid;

        Vector2 player = ctx.PlayerPosition.Value;
        Vector2 raw = ctx.PointerPosition.Value;
        
        var cfg = (AreaCircleConfig)config;
        
        _shape.Setup(cfg.XAngle, cfg.Range, cfg.Radius);
        
        Vector2 center =
            _shape.Clamp(player, raw);

        var cells =
            _world.GetCellsInRadius(center, cfg.Radius);

        if (cells.Count == 0)
            return TargetResult.Invalid;

        Vector2 dir = (center - player).normalized;

        return new TargetResult(
            cells, 
            player, 
            dir, 
            center);
    }
}