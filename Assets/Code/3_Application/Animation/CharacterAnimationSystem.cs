using System;
using UnityEngine;
public class CharacterAnimationSystem
{

  private ICharacterAnimationView _view;
  private readonly ICharacterAnimationLibrary _animationLibrary;
  public ICharacterAnimationLibrary AnimationLibrary => _animationLibrary;

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
      result.IsDead ? _animationLibrary.DeathTag : _animationLibrary.HitTag;

    var command = new CharacterAnimationCommand(
      tag.Id,
      tag.RuntimeTag,
      result.Direction);

    _view.Play(command);
  }


  public void ShowVisual()
  {
    _view.ShowVisual();
  }
  public void HideVisual()
  {
    _view.HideVisual();
  }

  public void Reset()
  {
    Debug.Log("Reset Animation");
    _view.ResetAnimation();
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