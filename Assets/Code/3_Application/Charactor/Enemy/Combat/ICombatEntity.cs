using UnityEngine;

public interface ICombatEntity
{
  Transform Transform { get; }

  float CombatRadius { get; }
}