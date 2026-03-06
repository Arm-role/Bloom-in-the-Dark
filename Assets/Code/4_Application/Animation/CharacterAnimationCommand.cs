using UnityEngine;

public readonly struct CharacterAnimationCommand
{
  public readonly CharacterAnimationTag Tag;
  public readonly Vector2 Direction;

  public CharacterAnimationCommand(
      CharacterAnimationTag tag,
      Vector2 direction)
  {
    Tag = tag;
    Direction = direction;
  }
}