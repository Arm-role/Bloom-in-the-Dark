
using System;
using UnityEngine;
public interface ICharacterAnimationView
{
  event Action RaiseImpact;
  event Action RaiseFinished;
  void Animation_Impact();
  void Animation_Finished();
  void SetMoveDirection(Vector2 moveDirection);
  void SetLookDirection(Vector2 lookDirection);
  bool Play(CharacterAnimationCommand command);
  void ShowVisual();
  void HideVisual();
  void ResetAnimation();
}