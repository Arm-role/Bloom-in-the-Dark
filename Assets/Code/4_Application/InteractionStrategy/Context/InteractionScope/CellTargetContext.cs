using System.Collections.Generic;
using UnityEngine;

public sealed class CellTargetContext : InteractionScope
{
    private Vector2? _pointer;
    public IReadOnlyList<WorldCell> Cells { get; }

    public override bool IsValid => Cells != null && Cells.Count > 0;
    public override Vector2? PointerPosition => _pointer;
    
    public CellTargetContext(
        IReadOnlyList<WorldCell> cells,
        Vector2? pointer)
    {
        Cells = cells;
        _pointer = pointer;
    }
}