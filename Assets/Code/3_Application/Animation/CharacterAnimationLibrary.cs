using UnityEngine;

[CreateAssetMenu(menuName = "Game/Animation/Character Animation Library")]
public class CharacterAnimationLibrary
    : ScriptableObject, ICharacterAnimationLibrary
{
  [SerializeField] private AnimationTag _hit;
  [SerializeField] private AnimationTag _death;
  [SerializeField] private AnimationTag _attack;
  [SerializeField] private AnimationTag _dash;
  [SerializeField] private AnimationTag _skill;

  public AnimationTag GetHitTag() => _hit;
  public AnimationTag GetDeathTag() => _death;
  public AnimationTag GetAttackTag() => _attack;
  public AnimationTag GetDashTag() => _dash;
  public AnimationTag GetSkillTag() => _skill;
}
