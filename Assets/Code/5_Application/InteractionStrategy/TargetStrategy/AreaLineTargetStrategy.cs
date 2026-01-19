using UnityEngine;

public class AreaLineTargetStrategy : ITargetStrategy
{
    private readonly AreaLineShape _shape;
    private readonly WorldTileManager _world;

    public AreaLineTargetStrategy(
        AreaLineShape shape,
        WorldTileManager world)
    {
        _shape = shape;
        _world = world;
    }

    public TargetResult Resolve(
        InteractionHandleContext ctx,
        ITargetingConfig config)
    {
        if (!ctx.PlayerPosition.HasValue ||
            !ctx.PointerPosition.HasValue)
            return TargetResult.Invalid;

        Vector2 origin = ctx.PlayerPosition.Value;
        Vector2 pointer = ctx.PointerPosition.Value;

        var cfg = (AreaLineConfig)config;

        _shape.Setup(
            cfg.XAngle,
            cfg.Length,
            cfg.Width);

        Vector2 end = _shape.ClampEnd(origin, pointer);
        Vector2 dir = (end - origin).normalized;

        var cells = _world.GetCellsFromArea(
            origin,
            Vector2.one * 2);

        Debug.Log("cells.Count : " + cells.Count);

        if (cells.Count == 0)
            return TargetResult.Invalid;

        return new TargetResult(
            cells,
            origin,
            dir,
            end);
    }
}