using UnityEngine;

[CreateAssetMenu(menuName = "Animation/Character Animation Library")]
public class CharacterAnimationLibrary
    : ScriptableObject, ICharacterAnimationLibrary
{
  [SerializeField] private AnimationTag _hit;
  [SerializeField] private AnimationTag _death;

  public AnimationTag GetHitTag() => _hit;
  public AnimationTag GetDeathTag() => _death;
}
