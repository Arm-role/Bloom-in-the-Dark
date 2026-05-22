using System;
using UnityEngine;
public interface IPlayerInput
{
    Vector2 MoveDirection { get; }
    Vector3 PointerWorldPosition { get; }

    bool IsPrimaryActionPressed { get; }
    bool IsPrimaryActionHeld { get; }
    bool IsPrimaryActionReleased { get; }

    bool IsSecondaryActionPressed { get; }
    bool IsSecondaryActionReleased { get; }
    bool IsSecondaryActionHeld { get; }

    bool IsSkillModifierPressed { get; }
    bool IsSkillModifierHeld { get; }
    bool IsSkillModifierReleased { get; }

    bool IsDashPressed { get; }
    bool IsInventoryToggle { get; }

    event Action OnPrimaryActionDown;
    event Action<float> OnScrollGlobal;
    event Action<int> OnHotbarSelect;
    event Action OnDash;
    event Action OnInventoryToggle;
    event Action OnPauseToggle;
    event Action OnInteract;
    event Action<bool> OnSkillModifier;

    float ScrollDelta { get; }
    int HotbarIndex { get; }

}
