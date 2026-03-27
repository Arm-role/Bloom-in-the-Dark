using UnityEngine;

[CreateAssetMenu(menuName = "Game/Animation/Character Animation Library")]
public class CharacterAnimationLibrary
    : ScriptableObject, ICharacterAnimationLibrary
{
  [SerializeField] private AnimationTag _idle;
  [SerializeField] private AnimationTag _hit;
  [SerializeField] private AnimationTag _death;
  [SerializeField] private AnimationTag _attack;
  [SerializeField] private AnimationTag _prepareDash;
  [SerializeField] private AnimationTag _dash;
  [SerializeField] private AnimationTag _endDash;
  [SerializeField] private AnimationTag _skill;

  public AnimationTag IdleTag { get => _idle; }
  public AnimationTag HitTag { get => _hit; }
  public AnimationTag DeathTag { get => _death; }
  public AnimationTag AttackTag { get => _attack; }
  public AnimationTag PrepareDashTag { get => _prepareDash; }
  public AnimationTag DashTag { get => _dash; }
  public AnimationTag EndDashTag { get => _endDash; }
  public AnimationTag SkillTag { get => _skill; }
}
