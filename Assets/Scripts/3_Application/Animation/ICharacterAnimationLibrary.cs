public interface ICharacterAnimationLibrary
{
  AnimationTag IdleTag { get; }
  AnimationTag HitTag { get; }
  AnimationTag DeathTag {get;}
  AnimationTag AttackTag {get;}
  AnimationTag PrepareDashTag {get;}
  AnimationTag DashTag {get;}
  AnimationTag EndDashTag {get;}
  AnimationTag SkillTag {get;}
  AnimationTag SlamWindupTag { get; }
  AnimationTag SlamRiseTag { get; }
  AnimationTag SlamFallTag { get; }
  AnimationTag SlamLandTag { get; }
  AnimationTag SlamRecoveryTag { get; }

  AnimationTag SpearSlamWindupTag { get; }
  AnimationTag SpearSlamRiseTag { get; }
  AnimationTag SpearSlamFallTag { get; }
  AnimationTag SpearSlamLandTag { get; }
  AnimationTag SpearSlamRecoveryTag { get; }

  AnimationTag SpearDiveWindupTag { get; }
  AnimationTag SpearDiveRiseTag { get; }
  AnimationTag SpearDivePeakTag { get; }
  AnimationTag SpearDiveFallTag { get; }
  AnimationTag SpearDiveLandTag { get; }
  AnimationTag SpearDiveRecoveryTag { get; }
}