using System;
using UnityEngine;
public class InputReader : MonoBehaviour, IPlayerInput
{
    private const float SCROLL_DEADZONE = 0.01f;

    // --- Interface Implementation ---
    public Vector2 MoveDirection { get; private set; }
    public Vector3 PointerWorldPosition { get; private set; }


    public bool IsPrimaryActionDown { get; private set; }
    public bool IsPrimaryActionPressed { get; private set; }
    public bool IsPrimaryActionReleased { get; private set; }

    public bool IsSecondaryActionDown { get; private set; }
    public bool IsSecondaryActionPressed { get; private set; }
    public bool IsSecondaryActionReleased { get; private set; }

    public float ScrollDelta { get; private set; }

    // --- Skill / Dash / Inventory ---
    public bool IsSkillModifierHeldDown { get; private set; }
    public bool IsSkillModifierHeld { get; private set; } 
    public bool IsSkillModifierHeldUp { get; private set; } 


    public bool IsDashPressed { get; private set; }       // Space
    public bool IsInventoryToggle { get; private set; }   // E

    public event Action OnPrimaryActionDown;
    public event Action OnSecondaryActionDown;
    public event Action<float> OnScrollGlobal;
    public event Action OnDash;
    public event Action OnInventoryToggle;
    public event Action<bool> OnSkillModifier;

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); 
        float vertical = Input.GetAxisRaw("Vertical");
        MoveDirection = new Vector2(horizontal, vertical).normalized;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        PointerWorldPosition = Camera.main.ScreenToWorldPoint(mousePos);

        //----Mouse----//
        IsPrimaryActionDown = Input.GetKeyDown(KeyCode.Mouse0);
        IsPrimaryActionPressed = Input.GetKey(KeyCode.Mouse0);
        IsPrimaryActionReleased = Input.GetKeyUp(KeyCode.Mouse0);

        IsSecondaryActionDown = Input.GetKeyDown(KeyCode.Mouse1);
        IsSecondaryActionPressed = Input.GetKey(KeyCode.Mouse1);
        IsSecondaryActionReleased = Input.GetKeyUp(KeyCode.Mouse1);

        if (IsPrimaryActionDown) OnPrimaryActionDown?.Invoke();
        if (IsSecondaryActionDown) OnSecondaryActionDown?.Invoke();

        //----Scroll----//
        float rawScroll = Input.GetAxis("Mouse ScrollWheel");
        ScrollDelta = Mathf.Abs(rawScroll) > SCROLL_DEADZONE ? Mathf.Sign(rawScroll) : 0f;
        if (Mathf.Abs(rawScroll) > SCROLL_DEADZONE)
            OnScrollGlobal?.Invoke(ScrollDelta);

        // --- Skill Modifier (Shift) ---
        bool shiftHeldDown = Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.RightShift);
        if (shiftHeldDown != IsSkillModifierHeld)
        {
            IsSkillModifierHeldDown = shiftHeldDown;
        }

        bool shiftHeld = Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightShift);
        if (shiftHeld != IsSkillModifierHeld)
        {
            IsSkillModifierHeld = shiftHeld;
            OnSkillModifier?.Invoke(IsSkillModifierHeld);
        }

        bool shiftHeldUp = Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.RightShift);
        if (shiftHeldUp != IsSkillModifierHeld)
        {
            IsSkillModifierHeldUp = shiftHeldUp;
        }

        // --- Dash (Space) ---
        IsDashPressed = Input.GetKeyDown(KeyCode.Space);
        if (IsDashPressed)
            OnDash?.Invoke();

        // --- Inventory (E) ---
        IsInventoryToggle = Input.GetKeyDown(KeyCode.E);
        if (IsInventoryToggle)
            OnInventoryToggle?.Invoke();
    }
}