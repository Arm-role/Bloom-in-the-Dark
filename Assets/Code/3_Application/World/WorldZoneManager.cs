using System;
using UnityEngine;
using System.Collections.Generic;

public class WorldZoneManager
{
  private float _lastRadius;
  private Vector3 _lastCenter;

  private readonly List<IWorldZone> _zones = new();
  private CircleZone _runtimeZone;

  public event Action<IWorldZone> ZoneChanged;
  public void ZoneChange(float radius, CellZoneFlags zoneFlags)
  {
    Vector3 center = new Vector3(0.5f, 0, 0);

    if (_runtimeZone == null)
    {
      _runtimeZone = new CircleZone(center, radius, zoneFlags);
      AddZone(_runtimeZone);
      ZoneChanged?.Invoke(_runtimeZone);

      return;
    }

    bool changed =
      _lastRadius != radius ||
      _lastCenter != center;

    if (!changed) return;

    _runtimeZone.Center = center;
    _runtimeZone.Radius = radius;
    _runtimeZone.SetFlags(zoneFlags);

    _lastRadius = radius;
    _lastCenter = center;

    ZoneChanged?.Invoke(_runtimeZone);
  }

  public void AddZone(IWorldZone zone)
  {
    _zones.Add(zone);
  }

  public void RemoveZone(IWorldZone zone)
  {
    _zones.Remove(zone);
  }

  public CellZoneFlags GetFlags(Vector3 worldPos)
  {
    CellZoneFlags flags = CellZoneFlags.None;

    foreach (var z in _zones)
    {
      if (z.Contains(worldPos))
        flags |= z.Flags;
    }

    return flags;
  }

  public IReadOnlyList<IWorldZone> GetZones()
  {
    return _zones;
  }
}