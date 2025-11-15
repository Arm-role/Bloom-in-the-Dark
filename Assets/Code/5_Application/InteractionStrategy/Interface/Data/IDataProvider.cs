using UnityEngine;

public interface IDataProvider
{
    bool IsValid { get; }
    Vector2? PointerPosition { get; }
}