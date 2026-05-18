using UnityEngine;

public struct CellObject
{
  public GameObject Object;
  public CellObjectFlags Flags;

  public CellObject(GameObject obj, CellObjectFlags flags)
  {
    Object = obj;
    Flags = flags;
  }

  public bool HasFlag(CellObjectFlags flag)
  {
    return (Flags & flag) != 0;
  }
}