using UnityEngine;

public class BossController : EnemyController
{
  private BossConfig BossConfig => config as BossConfig;

  public override void OnSpawnFromPool(GameObject ob)
  {
    base.OnSpawnFromPool(ob);
    AddBossSkills();
    WireBossEvents();
  }

  public override void OnReturnToPool(GameObject ob)
  {
    UnwireBossEvents();
    base.OnReturnToPool(ob);
  }

  private void AddBossSkills()
  {
    if (BossConfig == null) return;
    foreach (var def in BossConfig.bossSkills)
    {
      if (def == null) continue;
      AddSkill(def.Create(Sensor.targetMask));
    }
  }

  // ─── Spear Slam Events ───────────────────────────────────────────

  private void WireBossEvents()
  {
    Combat.OnPlaySpearSlamWindup += HandleSpearSlamWindupAnimation;
    Combat.OnPlaySpearSlamRise += HandleSpearSlamRiseAnimation;
    Combat.OnPlaySpearSlamFall += HandleSpearSlamFallAnimation;
    Combat.OnPlaySpearSlamLand += HandleSpearSlamLandAnimation;
    Combat.OnPlaySpearSlamRecovery += HandleSpearSlamRecoveryAnimation;

    Combat.OnPlaySpearDiveWindup += HandleSpearDiveWindupAnimation;
    Combat.OnPlaySpearDiveRise += HandleSpearDiveRiseAnimation;
    Combat.OnPlaySpearDivePeak += HandleSpearDivePeakAnimation;
    Combat.OnPlaySpearDiveFall += HandleSpearDiveFallAnimation;
    Combat.OnPlaySpearDiveLand += HandleSpearDiveLandAnimation;
    Combat.OnPlaySpearDiveRecovery += HandleSpearDiveRecoveryAnimation;
  }

  private void UnwireBossEvents()
  {
    Combat.OnPlaySpearSlamWindup -= HandleSpearSlamWindupAnimation;
    Combat.OnPlaySpearSlamRise -= HandleSpearSlamRiseAnimation;
    Combat.OnPlaySpearSlamFall -= HandleSpearSlamFallAnimation;
    Combat.OnPlaySpearSlamLand -= HandleSpearSlamLandAnimation;
    Combat.OnPlaySpearSlamRecovery -= HandleSpearSlamRecoveryAnimation;

    Combat.OnPlaySpearDiveWindup -= HandleSpearDiveWindupAnimation;
    Combat.OnPlaySpearDiveRise -= HandleSpearDiveRiseAnimation;
    Combat.OnPlaySpearDivePeak -= HandleSpearDivePeakAnimation;
    Combat.OnPlaySpearDiveFall -= HandleSpearDiveFallAnimation;
    Combat.OnPlaySpearDiveLand -= HandleSpearDiveLandAnimation;
    Combat.OnPlaySpearDiveRecovery -= HandleSpearDiveRecoveryAnimation;
  }

  private void HandleSpearSlamWindupAnimation()
  {
    var tag = AnimationSystem.AnimationLibrary.SpearSlamWindupTag;
    if (tag == null) return;
    AnimationSystem.Handle(new CharacterAnimationCommand(tag, State.MoveDirection));
  }

  private void HandleSpearSlamRiseAnimation()
  {
    var tag = AnimationSystem.AnimationLibrary.SpearSlamRiseTag;
    if (tag == null) return;
    AnimationSystem.Handle(new CharacterAnimationCommand(tag, State.MoveDirection));
  }

  private void HandleSpearSlamFallAnimation()
  {
    var tag = AnimationSystem.AnimationLibrary.SpearSlamFallTag;
    if (tag == null) return;
    AnimationSystem.Handle(new CharacterAnimationCommand(tag, State.MoveDirection, 0f));
  }

  private void HandleSpearSlamLandAnimation()
  {
    var tag = AnimationSystem.AnimationLibrary.SpearSlamLandTag;
    if (tag == null) return;
    AnimationSystem.Handle(new CharacterAnimationCommand(tag, State.MoveDirection, 0f));
  }

  private void HandleSpearSlamRecoveryAnimation()
  {
    var tag = AnimationSystem.AnimationLibrary.SpearSlamRecoveryTag;
    if (tag == null) return;
    AnimationSystem.Handle(new CharacterAnimationCommand(tag, State.MoveDirection, 0f));
  }

  // ─── Spear Dive Events ───────────────────────────────────────────

  private void HandleSpearDiveWindupAnimation()
  {
    var tag = AnimationSystem.AnimationLibrary.SpearDiveWindupTag;
    if (tag == null) return;
    AnimationSystem.Handle(new CharacterAnimationCommand(tag, State.MoveDirection));
  }

  private void HandleSpearDiveRiseAnimation()
  {
    var tag = AnimationSystem.AnimationLibrary.SpearDiveRiseTag;
    if (tag == null) return;
    AnimationSystem.Handle(new CharacterAnimationCommand(tag, State.MoveDirection));
  }

  private void HandleSpearDivePeakAnimation()
  {
    var tag = AnimationSystem.AnimationLibrary.SpearDivePeakTag;
    if (tag == null) return;
    AnimationSystem.Handle(new CharacterAnimationCommand(tag, State.MoveDirection, 0f));
  }

  private void HandleSpearDiveFallAnimation()
  {
    var tag = AnimationSystem.AnimationLibrary.SpearDiveFallTag;
    if (tag == null) return;
    AnimationSystem.Handle(new CharacterAnimationCommand(tag, State.MoveDirection, 0f));
  }

  private void HandleSpearDiveLandAnimation()
  {
    var tag = AnimationSystem.AnimationLibrary.SpearDiveLandTag;
    if (tag == null) return;
    AnimationSystem.Handle(new CharacterAnimationCommand(tag, State.MoveDirection, 0f));
  }

  private void HandleSpearDiveRecoveryAnimation()
  {
    var tag = AnimationSystem.AnimationLibrary.SpearDiveRecoveryTag;
    if (tag == null) return;
    AnimationSystem.Handle(new CharacterAnimationCommand(tag, State.MoveDirection, 0f));
  }
}
