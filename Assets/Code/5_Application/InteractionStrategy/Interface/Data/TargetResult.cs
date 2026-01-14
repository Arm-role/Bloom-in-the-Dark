using System.Collections.Generic;
using UnityEngine;

public readonly struct TargetResult
{
    public static TargetResult Invalid 
        => new TargetResult(
            null, 
            Vector2.zero, 
            Vector2.zero,  
            null);

    public readonly IReadOnlyList<WorldCell> Cells;
    public readonly Vector2 Origin;     
    public readonly Vector2 Direction;  
    public readonly object Extra;     

    public bool IsValid => Cells != null;

    public TargetResult(
        IReadOnlyList<WorldCell> cells,
        Vector2 origin,
        Vector2 direction,
        object extra)
    {
        Cells = cells;
        Origin = origin;
        Direction = direction;
        Extra = extra;
    }
}
