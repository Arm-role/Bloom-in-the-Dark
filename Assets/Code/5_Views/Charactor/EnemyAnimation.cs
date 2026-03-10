using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimation : MonoBehaviour, ICharacterAnimationView
{
  private static readonly int Horizontal = Animator.StringToHash("Horizontal");
  private static readonly int Vertical = Animator.StringToHash("Vertical");
  private static readonly int IsMoving = Animator.StringToHash("IsMoving");

  [SerializeField] private Animator _animator;
  [SerializeField] private GameObject _hpBar;

  [SerializeField] private AnimationTag[] animationTags;
  private Dictionary<GameTag, int> _map;

  public event Action RaiseImpact;
  public event Action RaiseFinished;

  void Awake()
  {
    _map = new();

    foreach (var tag in animationTags)
      _map[tag.RuntimeTag] = Animator.StringToHash(tag.Id);
  }
  private void Reset()
  {
    _animator = GetComponent<Animator>();
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
    int stateHash = -1;

    if (!_map.TryGetValue(command.Tag, out stateHash))
      return false;

    int layerIndex = 0;

    // เช็คว่ามี state นี้ใน layer หรือไม่
    if (!_animator.HasState(layerIndex, stateHash))
    {
      Debug.LogWarning($"Animation state not found: {command.Tag}");
      return false;
    }

    _animator.CrossFade(stateHash, 0.15f, layerIndex);
    return true;
  }

  private void ApplyFlip(Vector2 direction)
  {
    if (direction == Vector2.zero)
      return;

    Vector3 scale = _animator.transform.localScale;

    if (direction.x > 0)
      _animator.transform.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
    else if (direction.x < 0)
      _animator.transform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
  }

  public void ShowVisual()
  {
    var renderers = GetComponentsInChildren<SpriteRenderer>();

    foreach (var r in renderers)
      r.enabled = true;

    _hpBar.SetActive(true);
  }

  public void HideVisual()
  {
    var renderers = GetComponentsInChildren<SpriteRenderer>();

    foreach (var r in renderers)
      r.enabled = false;

    _hpBar.SetActive(false);
  }

  public void ResetAnimation()
  {
    _animator.Play("Helmet_Idle", 0, 0);
    _animator.Update(0f);
  }
}