using System;
using UnityEngine;

public interface IWorldZone
{
  Vector3 Center { get; }
  float Radius { get; }
  bool Contains(Vector3 worldPos);
  CellZoneFlags Flags { get; }
}

[Flags]
public enum CellZoneFlags
{
  None = 0,
  NoInteraction = 1 << 0,
  SafeZone = 1 << 1,
  CombatOnly = 1 << 2,
}
