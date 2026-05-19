using UnityEngine;

public readonly struct CharacterAnimationCommand
{
  public readonly AnimationTag Tag;
  public readonly Vector2 Direction;
  public readonly float TransitionDuration;

  public CharacterAnimationCommand(AnimationTag tag, Vector2 direction, float transitionDuration = 0.15f)
  {
    Tag = tag;
    Direction = direction;
    TransitionDuration = transitionDuration;
  }
}