using System;
using UnityEngine;
public class CharacterAnimationSystem
{
  private readonly ICharacterAnimationLibrary _animationLibrary;

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
    ICharacterAnimationLibrary animationLibrary)
  {
    _animationLibrary = animationLibrary;
  }

  public void Initializa(ICharacterAnimationView view)
  {
    _view = view;
  }

  public bool Handle(in CharacterAnimationCommand command)
    => _view.Play(command);

  public void SetMoveDirection(Vector2 vector)
  => _view.SetMoveDirection(vector);

  public void SetLookDirection(Vector2 vector)
    => _view.SetLookDirection(vector);



  public void HandleDamage(CharacterDamageResult result)
  {
    AnimationTag tag =
      result.IsDead ? _animationLibrary.GetDeathTag() : _animationLibrary.GetHitTag();

    var command = new CharacterAnimationCommand(
      tag.Id,
      tag.RuntimeTag,
      result.Direction);

    _view.Play(command);
  }

  public void PlayAttack(Vector2 dir)
  {
    var tag = _animationLibrary.GetAttackTag();

    var command = new CharacterAnimationCommand(
        tag.Id,
        tag.RuntimeTag,
        dir);

    _view.Play(command);
  }
  public void PlayDash(Vector2 dir)
  {
    var tag = _animationLibrary.GetDashTag();

    var command = new CharacterAnimationCommand(
        tag.Id,
        tag.RuntimeTag,
        dir);

    _view.Play(command);
  }
  public void PlaySkill(Vector2 dir)
  {
    var tag = _animationLibrary.GetSkillTag();

    var command = new CharacterAnimationCommand(
        tag.Id,
        tag.RuntimeTag,
        dir);

    _view.Play(command);
  }
}

public enum EAnimationIntent
{
  None,
  Attack,
  Skill,
  Dash,
  Hit,
  Death
}