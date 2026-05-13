using System;

public readonly struct GameTag : IEquatable<GameTag>
{
  public readonly int Hash;

  public GameTag(string id)
  {
    Hash = id.GetHashCode();
  }

  public bool Equals(GameTag other) => Hash == other.Hash;
  public override int GetHashCode() => Hash;
}