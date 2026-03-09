using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Animation/Character Animation Config")]
public class CharacterAnimationConfig : ScriptableObject
{
  [SerializeField] private List<CharacterAnimationRule> _rules;

  public IReadOnlyList<CharacterAnimationRule> Rules => _rules;
}
