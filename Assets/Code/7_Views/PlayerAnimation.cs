using UnityEngine;

public class PlayerAnimation : MonoBehaviour, IPlayerAnimationView
{
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int Speed = Animator.StringToHash("Speed");

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetMoveDirection(Vector2 moveDirection)
    {
        _animator.SetFloat(Horizontal, moveDirection.x);
        _animator.SetFloat(Vertical, moveDirection.y);
        _animator.SetFloat(Speed, moveDirection.sqrMagnitude);
    }
}
