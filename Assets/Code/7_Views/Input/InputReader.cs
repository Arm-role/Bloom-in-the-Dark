using System;
using UnityEngine;
public class InputReader : MonoBehaviour, IPlayerInput
{
    private const float SCROLL_DEADZONE = 0.01f;

    // --- Interface Implementation ---
    public Vector2 MoveDirection { get; private set; }
    public Vector3 PointerWorldPosition { get; private set; }


    public bool IsPrimaryActionPressed { get; private set; }
    public bool IsPrimaryActionHeld { get; private set; }
    public bool IsPrimaryActionReleased { get; private set; }

    public bool IsSecondaryActionPressed { get; private set; }
    public bool IsSecondaryActionHeld { get; private set; }
    public bool IsSecondaryActionReleased { get; private set; }

    public float ScrollDelta { get; private set; }
    public int HotbarIndex { get; private set; }

    private const int HOTBAR_MIN = 0;
    private const int HOTBAR_MAX = 5;

    // --- Skill / Dash / Inventory ---
    public bool IsSkillModifierPressed { get; private set; }
    public bool IsSkillModifierHeld { get; private set; }
    public bool IsSkillModifierReleased { get; private set; }


    public bool IsDashPressed { get; private set; }       // Space
    public bool IsInventoryToggle { get; private set; }   // E

    public event Action OnPrimaryActionDown;
    public event Action OnSecondaryActionDown;
    public event Action<float> OnScrollGlobal;
    public event Action<int> OnHotbarSelect;
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

        IsPrimaryActionPressed = Input.GetKeyDown(primaryKey);
        IsPrimaryActionHeld = Input.GetKey(primaryKey);
        IsPrimaryActionReleased = Input.GetKeyUp(primaryKey);

        KeyCode secondaryKey = KeyCode.Mouse1;

        IsSecondaryActionPressed = Input.GetKeyDown(secondaryKey);
        IsSecondaryActionHeld = Input.GetKey(secondaryKey);
        IsSecondaryActionReleased = Input.GetKeyUp(secondaryKey);

        if (IsPrimaryActionPressed) OnPrimaryActionDown?.Invoke();
        if (IsSecondaryActionPressed) OnSecondaryActionDown?.Invoke();

        //----Scroll----//
        float rawScroll = Input.GetAxis("Mouse ScrollWheel");
        ScrollDelta = Mathf.Abs(rawScroll) > SCROLL_DEADZONE ? Mathf.Sign(rawScroll) : 0f;
        if (Mathf.Abs(rawScroll) > SCROLL_DEADZONE)
            OnScrollGlobal?.Invoke(ScrollDelta);

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Q))
        {
            OnHotbarSelect?.Invoke(-1);
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.E))
        {
            OnHotbarSelect?.Invoke(+1);
        }

        for (int i = 0; i <= HOTBAR_MAX; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                OnHotbarSelect?.Invoke(i);
            }
        }

        // --- Skill Modifier (Shift) ---
        KeyCode shiftHeldKey1 = KeyCode.LeftShift;
        KeyCode shiftHeldKey2 = KeyCode.RightShift;

        bool shiftHeldDown = Input.GetKeyDown(shiftHeldKey1) || Input.GetKeyDown(shiftHeldKey2);
        if (shiftHeldDown != IsSkillModifierHeld)
        {
            IsSkillModifierPressed = shiftHeldDown;
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
            IsSkillModifierReleased = shiftHeldUp;
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