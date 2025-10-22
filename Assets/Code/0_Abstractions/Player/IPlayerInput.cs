using System;
using UnityEngine;
public interface IPlayerInput
{
    Vector2 MoveDirection { get; }
    Vector3 PointerWorldPosition { get; }

    bool IsPrimaryActionDown { get; }
    bool IsPrimaryActionPressed { get; }
    bool IsPrimaryActionReleased { get; }

    event Action OnPrimaryActionDown;
    event Action OnSecondaryActionDown;
    event Action<float> OnScrollGlobal;

    bool IsSecondaryActionDown { get; }
    bool IsSecondaryActionPressed { get; }
    bool IsSecondaryActionReleased { get; }

    float ScrollDelta { get; }
}
