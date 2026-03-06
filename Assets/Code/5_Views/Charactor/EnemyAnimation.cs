using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimation : MonoBehaviour, ICharacterAnimationView
{
  private static readonly int Horizontal = Animator.StringToHash("Horizontal");
  private static readonly int Vertical = Animator.StringToHash("Vertical");
  private static readonly int IsMoving = Animator.StringToHash("IsMoving");

  [SerializeField] private Animator _animator;
  [SerializeField] private Transform visual;

  public event Action RaiseImpact;
  public event Action RaiseFinished;

  private void Reset()
  {
    _animator = GetComponent<Animator>();

    if (visual == null && transform.childCount > 0)
      visual = transform.GetChild(0);
  }

  public void Animation_Impact() => RaiseImpact?.Invoke();
  public void Animation_Finished() => RaiseFinished?.Invoke();

  public void SetMoveDirection(Vector2 moveDirection)
  {
    _animator.SetFloat(Horizontal, moveDirection.x);
    _animator.SetFloat(Vertical, moveDirection.y);
    _animator.SetBool(IsMoving, moveDirection.sqrMagnitude > 0.01f);

    ApplyFlip(moveDirection);
  }

  public void SetLookDirection(Vector2 lookDirection)
  {
    if (lookDirection == Vector2.zero)
      return;

    _animator.SetFloat(Horizontal, lookDirection.x);
    _animator.SetFloat(Vertical, lookDirection.y);

    ApplyFlip(lookDirection);
  }

  public bool Play(CharacterAnimationCommand command)
  {
    if (command.Tag == null)
      return false;

    int layerIndex = 0;
    int stateHash = command.Tag.Hash;

    // เช็คว่ามี state นี้ใน layer หรือไม่
    if (!_animator.HasState(layerIndex, stateHash))
    {
      Debug.LogWarning($"Animation state not found: {command.Tag.name}");
      return false;
    }

    _animator.CrossFade(stateHash, 0.15f, layerIndex);
    return true;
  }

  private void ApplyFlip(Vector2 direction)
  {
    if (visual == null || direction == Vector2.zero)
      return;

    Vector3 scale = visual.localScale;

    if (direction.x > 0)
      visual.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
    else if (direction.x < 0)
      visual.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
  }
}