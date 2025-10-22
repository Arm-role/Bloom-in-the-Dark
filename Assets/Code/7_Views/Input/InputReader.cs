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

    public event Action OnPrimaryActionDown;
    public event Action OnSecondaryActionDown;
    public event Action<float> OnScrollGlobal;

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

        //----Scroll----//
        float rawScroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(rawScroll) > SCROLL_DEADZONE)
        {
            ScrollDelta = Mathf.Sign(rawScroll);
        }
        else
        {
            ScrollDelta = 0f;
        }
    }
}