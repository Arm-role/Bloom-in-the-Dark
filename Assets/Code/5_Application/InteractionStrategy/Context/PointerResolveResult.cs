using UnityEngine;

public readonly struct PointerResolveResult
{
    public readonly Vector2 Raw;     // จาก mouse / touch / analog stick
    public readonly Vector2 Resolve; // หลังผ่าน logic ของ strategy (limit, snap, etc.)
    public readonly bool IsValid;

    public PointerResolveResult(Vector2 raw, Vector2 resolved, bool valid = true)
    {
        Raw = raw;
        Resolve = resolved;
        IsValid = valid;
    }
}
