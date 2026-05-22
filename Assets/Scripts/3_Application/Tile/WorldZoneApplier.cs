#nullable enable
using System;

// Applies world-zone flags to cells. Owns the WorldZoneManager subscription
// and refreshes affected cells whenever a zone changes.
public sealed class WorldZoneApplier : IDisposable
{
  private readonly WorldZoneManager _zoneManager;
  private readonly WorldCellRegistry _registry;
  private readonly GridSpatialQuery _spatialQuery;

  public WorldZoneApplier(
    WorldZoneManager zoneManager,
    WorldCellRegistry registry,
    GridSpatialQuery spatialQuery)
  {
    _zoneManager = zoneManager;
    _registry = registry;
    _spatialQuery = spatialQuery;

    _zoneManager.ZoneChanged += OnZoneChanged;
  }

  public void Dispose()
    => _zoneManager.ZoneChanged -= OnZoneChanged;

  // Applies current zone flags to one cell — used when a cell is first created.
  public void RefreshCell(WorldCell cell)
    => cell.SetZoneFlags(_zoneManager.GetFlags(cell.WorldCenter));

  public void RefreshAll()
  {
    foreach (var cell in _registry.All)
      RefreshCell(cell);
  }

  public void ApplyZone(IWorldZone zone)
  {
    foreach (var cell in _spatialQuery.GetCellsInRadius(zone.Center, zone.Radius))
      RefreshCell(cell);
  }

  private void OnZoneChanged(IWorldZone zone) => ApplyZone(zone);
}
