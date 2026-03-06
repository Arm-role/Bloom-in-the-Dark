using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour, ICharacterAnimationView
{
  private static readonly int Horizontal = Animator.StringToHash("Horizontal");
  private static readonly int Vertical = Animator.StringToHash("Vertical");
  private static readonly int IsMoving = Animator.StringToHash("IsMoving");

  [SerializeField] private Animator _animator;

  public event Action RaiseImpact;
  public event Action RaiseFinished;

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
    if (direction == Vector2.zero) return;

    Vector3 scale = _animator.transform.localScale;

    if (direction.x > 0)
      _animator.transform.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
    else if (direction.x < 0)
      _animator.transform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
  }
}