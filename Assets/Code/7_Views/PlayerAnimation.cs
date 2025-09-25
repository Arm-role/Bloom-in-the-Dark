using UnityEngine;

public class PlayerAnimation : MonoBehaviour, IPlayerAnimationView
{
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");

    [SerializeField]
    private Animator _animator;

    public void SetMoveDirection(Vector2 moveDirection)
    {
        _animator.SetFloat(Horizontal, moveDirection.x);
        _animator.SetFloat(Vertical, moveDirection.y);

        if (moveDirection.x > 0)
        {
            Vector3 scale = _animator.transform.localScale;
            if (_animator.transform.localScale.x < 0) return;
            _animator.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
        }
        else
        {
            Vector3 scale = _animator.transform.localScale;
            _animator.transform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
        }
    }
}
