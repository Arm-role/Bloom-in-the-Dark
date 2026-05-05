using UnityEngine;

public class ConeTargetStrategy : ITargetStrategy
{
  private readonly ConeShape _shape;
  private readonly WorldTileManager _world;

  public ConeTargetStrategy(
      ConeShape shape,
      WorldTileManager world)
  {
    _shape = shape;
    _world = world;
  }

  public TargetResult Resolve(InteractionHandleContext ctx, ITargetingConfig config)
  {
    if (!ctx.PlayerPosition.HasValue || !ctx.PointerPosition.HasValue)
      return TargetResult.Invalid;

    Vector2 player = ctx.PlayerPosition.Value;
    Vector2 pointer = ctx.PointerPosition.Value;

    var cfg = (ConeConfig)config;

    _shape.Setup(cfg.XAngle, cfg.Range, cfg.AngleDeg);

    // direction จาก player → pointer (world space)
    Vector2 dir = _shape.GetWorldDirection(player, pointer);

    // ดึง cells ใน radius ก่อน (broad-phase เหมือน AreaCircle ใช้ GetCellsInRadius)
    var cells = _world.GetCellsInRadius(player, cfg.Range);

    // narrow-phase: กรองเฉพาะ cell ที่อยู่ใน cone จริงๆ
    var coneCells = new System.Collections.Generic.List<WorldCell>();
    foreach (var cell in cells)
    {
      if (_shape.IsInside(player, dir, cell.WorldCenter))
        coneCells.Add(cell);
    }

    if (coneCells.Count == 0)
      return TargetResult.Invalid;

    return new TargetResult(
        coneCells,
        player,
        dir,
        player   // cone origin = player position (ไม่มี separate center ต่างจาก AreaCircle)
    );
  }
}