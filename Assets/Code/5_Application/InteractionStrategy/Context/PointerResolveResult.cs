using UnityEngine;

public readonly struct PointerResolveResult
{
    public readonly Vector2 RawPointer;     // จาก mouse / touch / analog stick
    public readonly Vector2 ResolvedPointer; // หลังผ่าน logic ของ strategy (limit, snap, etc.)
    public readonly bool IsValid;

    public PointerResolveResult(Vector2 raw, Vector2 resolved, bool valid = true)
    {
        RawPointer = raw;
        ResolvedPointer = resolved;
        IsValid = valid;
    }
}
