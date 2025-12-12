using System;
using UnityEngine;
public interface IPlayerInput
{
    Vector2 MoveDirection { get; }
    Vector3 PointerWorldPosition { get; }

    bool IsPrimaryActionDown { get; }
    bool IsPrimaryActionPressed { get; }
    bool IsPrimaryActionReleased { get; }

    bool IsSecondaryActionDown { get; }
    bool IsSecondaryActionReleased { get; }
    bool IsSecondaryActionPressed { get; }

    bool IsSkillModifierHeldDown { get; }
    bool IsSkillModifierHeld { get; }
    bool IsSkillModifierHeldUp { get; }

    bool IsDashPressed { get; }
    bool IsInventoryToggle { get; }

    event Action OnPrimaryActionDown;
    event Action<float> OnScrollGlobal;
    event Action<int> OnHotbarSelect;
    event Action OnDash;
    event Action OnInventoryToggle;
    event Action<bool> OnSkillModifier;

    float ScrollDelta { get; }
    int HotbarIndex { get; }

}
