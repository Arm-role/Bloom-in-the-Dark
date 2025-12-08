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
        KeyCode primaryKey = KeyCode.Mouse0;

        IsPrimaryActionDown = Input.GetKeyDown(primaryKey);
        IsPrimaryActionPressed = Input.GetKey(primaryKey);
        IsPrimaryActionReleased = Input.GetKeyUp(primaryKey);

        KeyCode secondaryKey = KeyCode.Mouse1;

        IsSecondaryActionDown = Input.GetKeyDown(secondaryKey);
        IsSecondaryActionPressed = Input.GetKey(secondaryKey);
        IsSecondaryActionReleased = Input.GetKeyUp(secondaryKey);

        if (IsPrimaryActionDown) OnPrimaryActionDown?.Invoke();
        if (IsSecondaryActionDown) OnSecondaryActionDown?.Invoke();

        //----Scroll----//
        float rawScroll = Input.GetAxis("Mouse ScrollWheel");
        ScrollDelta = Mathf.Abs(rawScroll) > SCROLL_DEADZONE ? Mathf.Sign(rawScroll) : 0f;
        if (Mathf.Abs(rawScroll) > SCROLL_DEADZONE)
            OnScrollGlobal?.Invoke(ScrollDelta);

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Q))
        {
            ScrollDelta--;
            OnScrollGlobal?.Invoke(ScrollDelta);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.E))
        {
            ScrollDelta++;
            OnScrollGlobal?.Invoke(ScrollDelta);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ScrollDelta = 0;
            OnScrollGlobal?.Invoke(ScrollDelta);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ScrollDelta = 1;
            OnScrollGlobal?.Invoke(ScrollDelta);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ScrollDelta = 2;
            OnScrollGlobal?.Invoke(ScrollDelta);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ScrollDelta = 3;
            OnScrollGlobal?.Invoke(ScrollDelta);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ScrollDelta = 4;
            OnScrollGlobal?.Invoke(ScrollDelta);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ScrollDelta = 5;
            OnScrollGlobal?.Invoke(ScrollDelta);
        }

        // --- Skill Modifier (Shift) ---
        KeyCode shiftHeldKey1 = KeyCode.LeftShift;
        KeyCode shiftHeldKey2 = KeyCode.RightShift;

        bool shiftHeldDown = Input.GetKeyDown(shiftHeldKey1) || Input.GetKeyDown(shiftHeldKey2);
        if (shiftHeldDown != IsSkillModifierHeld)
        {
            IsSkillModifierHeldDown = shiftHeldDown;
        }

        bool shiftHeld = Input.GetKey(shiftHeldKey1) || Input.GetKey(shiftHeldKey2);
        if (shiftHeld != IsSkillModifierHeld)
        {
            IsSkillModifierHeld = shiftHeld;
            OnSkillModifier?.Invoke(IsSkillModifierHeld);
        }

        bool shiftHeldUp = Input.GetKeyUp(shiftHeldKey1) || Input.GetKeyUp(shiftHeldKey2);
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