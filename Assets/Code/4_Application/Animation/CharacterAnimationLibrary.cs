using UnityEngine;

[CreateAssetMenu(menuName = "Animation/Character Animation Library")]
public class CharacterAnimationLibrary
    : ScriptableObject, ICharacterAnimationLibrary
{
  [SerializeField] private CharacterAnimationTag _hit;
  [SerializeField] private CharacterAnimationTag _death;

  public CharacterAnimationTag GetHitTag() => _hit;
  public CharacterAnimationTag GetDeathTag() => _death;
}
