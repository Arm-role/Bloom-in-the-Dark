using UnityEngine;

public class PlayerAnimation : MonoBehaviour, ICharacterAnimationView
{
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");

    [SerializeField]
    private Animator _animator;

    public void SetMoveDirection(Vector2 moveDirection)
    {
        _animator.SetFloat(Horizontal, moveDirection.x);
        _animator.SetFloat(Vertical, moveDirection.y);
        _animator.SetBool(IsMoving, moveDirection.sqrMagnitude > 0.01f);

        ApplyFlip(moveDirection);
    }
    public void SetLookirection(Vector2 lookDirection)
    {
        _animator.SetFloat(Horizontal, lookDirection.x);
        _animator.SetFloat(Vertical, lookDirection.y);

        ApplyFlip(lookDirection);
    }

    private void ApplyFlip(Vector2 moveDirection)
    {
        if (moveDirection == Vector2.zero) return;

        Vector3 scale = _animator.transform.localScale;

        if (moveDirection.x > 0)
            _animator.transform.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
        else if (moveDirection.x < 0)
            _animator.transform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
    }

    public void PlayAnimation(string key)
    {
        Debug.Log("PlayAnimation");
    }

    public void PlayAttack(string attackKey)
    {
        Debug.Log("PlayAttack");
    }

    public void PlayDash()
    {
        Debug.Log("PlayDash");
    }

    public void PlaySlam()
    {
        Debug.Log("PlaySlam");
    }

    public void PlayHit()
    {
        //Debug.Log("PlayHit");
    }

    public void PlayDeath()
    {
        Debug.Log("PlayDeath");
    }
}
