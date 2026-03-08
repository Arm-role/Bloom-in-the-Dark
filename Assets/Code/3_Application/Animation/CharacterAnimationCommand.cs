using UnityEngine;

public readonly struct CharacterAnimationCommand
{
  public readonly string TagName;
  public readonly GameTag Tag;
  public readonly Vector2 Direction;

  public CharacterAnimationCommand(string tagName, GameTag tag, Vector2 direction)
  {
    TagName = tagName;
    Tag = tag;
    Direction = direction;
  }
}