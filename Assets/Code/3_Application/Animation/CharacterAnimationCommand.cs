using UnityEngine;

public readonly struct CharacterAnimationCommand
{
  public readonly AnimationTag Tag;   
  public readonly Vector2 Direction;

  public CharacterAnimationCommand(AnimationTag tag, Vector2 direction)
  {
    Tag = tag;
    Direction = direction;
  }
}