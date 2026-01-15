using System.Collections.Generic;
using UnityEngine;

public class GridTargetStrategy : ITargetStrategy
{
    private readonly WorldTileManager _world;

    public GridTargetStrategy(
        WorldTileManager world) => _world = world;

    public TargetResult Resolve(InteractionHandleContext ctx, ITargetingConfig config)
    {
        if (!ctx.PlayerPosition.HasValue || !ctx.PointerPosition.HasValue)
            return TargetResult.Invalid;

        Vector2 player = ctx.PlayerPosition.Value;
        Vector2 rawPointer = ctx.PointerPosition.Value;

        var cfg = (GridTargetConfig)config;

        Vector2 centerWorld = ResolveCenter(ctx, rawPointer, cfg.MaxRange, cfg.Size);

        Vector3Int centerCell =
            _world.GridConverter.WorldToCell(centerWorld);

        var cells = CollectCells(centerCell, cfg.Size);

        if (cells.Count == 0)
            return TargetResult.Invalid;

        Vector2 dir = (centerWorld - player).normalized;

        return new TargetResult(
            cells,
            player,
            dir,
            centerWorld);
    }

    // -------------------------------------------------

    private Vector2 ResolveCenter(
        InteractionHandleContext ctx,
        Vector2 rawPointer,
        float maxRange,
        Vector2Int size)
    {
        Vector2 player = ctx.PlayerPosition.Value;
        float dist = Vector2.Distance(player, rawPointer);

        // pointer อยู่ใน range
        if (dist <= maxRange)
            return rawPointer;

        // ---------- out of range ----------
        // Case A: single cell → หน้า player
        if (size == Vector2Int.one)
        {
            Vector2 dir =
                ctx.PlayerDirection?.normalized
                ?? (rawPointer - player).normalized;

            return player + dir;
        }

        // Case B: multi cell → clamp ตามทิศ pointer
        Vector2 pointerDir = (rawPointer - player).normalized;
        return player + pointerDir * maxRange;
    }

    private List<WorldCell> CollectCells(Vector3Int center, Vector2Int size)
    {
        var result = new List<WorldCell>(size.x * size.y);

        int offsetX = size.x / 2;
        int offsetY = size.y / 2;

        for (int x = 0; x < size.x; x++)
        for (int y = 0; y < size.y; y++)
        {
            var pos = new Vector3Int(
                center.x + x - offsetX,
                center.y + y - offsetY,
                0);

            var cell = _world.GetCell(pos);
            if (cell != null)
                result.Add(cell);
        }

        return result;
    }
}