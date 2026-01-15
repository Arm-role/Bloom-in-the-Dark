using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimation : MonoBehaviour, ICharacterAnimationView
{
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");
    private static readonly int DashTrigger = Animator.StringToHash("Dash");
    private static readonly int SlamTrigger = Animator.StringToHash("Slam");
    private static readonly int HitTrigger = Animator.StringToHash("Hit");
    private static readonly int DeathTrigger = Animator.StringToHash("Death");

    [SerializeField] private Animator _animator;
    [SerializeField] private Transform visual; // child that holds sprite/graphics

    private void Reset()
    {
        _animator = GetComponent<Animator>();
        if (visual == null && transform.childCount > 0)
            visual = transform.GetChild(0);
    }

    public void SetMoveDirection(Vector2 moveDirection)
    {
        _animator.SetFloat(Horizontal, moveDirection.x);
        _animator.SetFloat(Vertical, moveDirection.y);
        _animator.SetBool(IsMoving, moveDirection.sqrMagnitude > 0.01f);
        ApplyFlip(moveDirection);
    }

    public void SetLookirection(Vector2 lookDirection)
    {
        if (lookDirection == Vector2.zero) return;
        _animator.SetFloat(Horizontal, lookDirection.x);
        _animator.SetFloat(Vertical, lookDirection.y);
        ApplyFlip(lookDirection);
    }

    public void PlayAnimation(string key)
    {
        // generic: map some keys to triggers if needed
        switch (key)
        {
            case "melee": _animator.SetTrigger(AttackTrigger); break;
            case "slam": _animator.SetTrigger(SlamTrigger); break;
            default: _animator.SetTrigger(AttackTrigger); break;
        }
    }

    public void PlayAttack(string attackKey) => PlayAnimation(attackKey);
    public void PlayDash() => _animator.SetTrigger(DashTrigger);
    public void PlaySlam() => _animator.SetTrigger(SlamTrigger);
    public void PlayHit() => _animator.SetTrigger(HitTrigger);
    public void PlayDeath() => _animator.SetTrigger(DeathTrigger);

    private void ApplyFlip(Vector2 moveDirection)
    {
        if (visual == null) return;
        if (moveDirection == Vector2.zero) return;

        Vector3 scale = visual.localScale;
        if (moveDirection.x > 0)
            visual.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
        else if (moveDirection.x < 0)
            visual.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
    }
}
