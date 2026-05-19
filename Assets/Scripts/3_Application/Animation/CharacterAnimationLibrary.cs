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
  [SerializeField] private AnimationTag _slamWindupTag;
  [SerializeField] private AnimationTag _slamRiseTag;
  [SerializeField] private AnimationTag _slamFallTag;
  [SerializeField] private AnimationTag _slamLandTag;
  [SerializeField] private AnimationTag _slamRecoveryTag;

  [Header("Spear Slam")]
  [SerializeField] private AnimationTag _spearSlamWindupTag;
  [SerializeField] private AnimationTag _spearSlamRiseTag;
  [SerializeField] private AnimationTag _spearSlamFallTag;
  [SerializeField] private AnimationTag _spearSlamLandTag;
  [SerializeField] private AnimationTag _spearSlamRecoveryTag;

  [Header("Spear Dive")]
  [SerializeField] private AnimationTag _spearDiveWindupTag;
  [SerializeField] private AnimationTag _spearDiveRiseTag;
  [SerializeField] private AnimationTag _spearDivePeakTag;
  [SerializeField] private AnimationTag _spearDiveFallTag;
  [SerializeField] private AnimationTag _spearDiveLandTag;
  [SerializeField] private AnimationTag _spearDiveRecoveryTag;


  public AnimationTag IdleTag { get => _idle; }
  public AnimationTag HitTag { get => _hit; }
  public AnimationTag DeathTag { get => _death; }
  public AnimationTag AttackTag { get => _attack; }
  public AnimationTag PrepareDashTag { get => _prepareDash; }
  public AnimationTag DashTag { get => _dash; }
  public AnimationTag EndDashTag { get => _endDash; }
  public AnimationTag SkillTag { get => _skill; }
  public AnimationTag SlamWindupTag { get => _slamWindupTag; }
  public AnimationTag SlamRiseTag { get => _slamRiseTag; }
  public AnimationTag SlamFallTag { get => _slamFallTag; }
  public AnimationTag SlamLandTag { get => _slamLandTag; }
  public AnimationTag SlamRecoveryTag { get => _slamRecoveryTag; }

  public AnimationTag SpearSlamWindupTag => _spearSlamWindupTag;
  public AnimationTag SpearSlamRiseTag => _spearSlamRiseTag;
  public AnimationTag SpearSlamFallTag => _spearSlamFallTag;
  public AnimationTag SpearSlamLandTag => _spearSlamLandTag;
  public AnimationTag SpearSlamRecoveryTag => _spearSlamRecoveryTag;

  public AnimationTag SpearDiveWindupTag => _spearDiveWindupTag;
  public AnimationTag SpearDiveRiseTag => _spearDiveRiseTag;
  public AnimationTag SpearDivePeakTag => _spearDivePeakTag;
  public AnimationTag SpearDiveFallTag => _spearDiveFallTag;
  public AnimationTag SpearDiveLandTag => _spearDiveLandTag;
  public AnimationTag SpearDiveRecoveryTag => _spearDiveRecoveryTag;
}
