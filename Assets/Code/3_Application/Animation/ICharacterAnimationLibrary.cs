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
}