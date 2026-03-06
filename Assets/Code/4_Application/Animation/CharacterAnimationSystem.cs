using System;
using UnityEngine;

public class CharacterAnimationSystem
{
  private readonly ICharacterAnimationLibrary _animationLibrary;
  private readonly CharacterAnimationTagService _tagService;

  private ICharacterAnimationView _view;

  public event Action RaiseImpact
  {
    add => _view.RaiseImpact += value;
    remove => _view.RaiseImpact -= value;
  }

  public event Action RaiseFinished
  {
    add => _view.RaiseFinished += value;
    remove => _view.RaiseFinished -= value;
  }

  public CharacterAnimationSystem(
    ICharacterAnimationLibrary animationLibrary,
    CharacterAnimationTagService tagService)
  {
    _animationLibrary = animationLibrary;
    _tagService = tagService;
  }

  public void Initializa(ICharacterAnimationView view)
  {
    _view = view;
  }

  public bool Handle(in InteractionExecutionResult result)
  {
    if (_tagService.TryResolve(result, out var tag))
    {
      var command = new CharacterAnimationCommand(
          tag,
          result.LookDirection);

      return _view.Play(command);
    }

    return false;
  }

  public void HandleDamage(CharacterDamageResult result)
  {
    CharacterAnimationTag tag =
        result.IsDead ? _animationLibrary.GetDeathTag() : _animationLibrary.GetHitTag();

    var command = new CharacterAnimationCommand(
        tag,
        result.Direction);

    _view.Play(command);
  }

  public void SetMoveDirection(Vector2 vector)
    => _view.SetMoveDirection(vector);

  public void SetLookDirection(Vector2 vector)
    => _view.SetLookDirection(vector);
}




[Serializable]
public class PlayerAnimationContext
{
  public EInteractionIntentType Intent;
  public EItemCategory Category;
  public EItemRole Role;
  public ETargetType TargetMask;

  public int EnergyCost;
  public int ItemCost;
  public float Cooldown;
}

[Serializable]
public class PlayerAnimationRule
{
  public EInteractionIntentType Intent;
  public EItemCategory Category;
  public EItemRole Role;
  public ETargetType TargetMask;

  public int EnergyCost;
  public int ItemCost;
  public float Cooldown;
}
