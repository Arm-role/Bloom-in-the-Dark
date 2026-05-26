#nullable enable

using UnityEngine;

public sealed class SkillController
{
  private readonly SpawnerHandle _spawner;
  private readonly SkillSpawnController _skillSpawn;
  private readonly SkillSelfController _skillSelf;
  private readonly IAudioService? _audio;

  public SkillController(SpawnerHandle spawner, IEnergyable energyable, IAudioService? audio = null)
  {
    _spawner = spawner;
    _audio = audio;
    _skillSpawn = new SkillSpawnController(_spawner);
    _skillSelf = new SkillSelfController(energyable);
  }

  public void SetChannelContext(IPlayerInput input, IPlayerInteractor interactor)
    => _skillSpawn.SetChannelContext(input, interactor);

  public void ActiveSelfSkill(
   ISkillDataPayload payload, InteractionIntent intent)
    => _skillSelf.Use(payload, intent);

  public void ActiveSkill(
    ISkillDataPayload payload, GameObject owner, InteractionIntent intent, ISkillDefinition skillDefinition, Vector2 targetPos)
  {
    PlayCastSfx(skillDefinition, owner);
    _skillSpawn.ActiveSkill(payload, owner, intent, skillDefinition, targetPos);
  }

  public void ActiveSkill(
    ISkillDataPayload payload, GameObject owner, InteractionIntent intent, ISkillDefinition skillDefinition, Vector2 targetPos, Vector2 direction)
  {
    PlayCastSfx(skillDefinition, owner);
    _skillSpawn.ActiveSkill(payload, owner, intent, skillDefinition, targetPos, direction);
  }

  // เล่นเสียงตอน cast (swing/launch) — fire ก่อน async spawn เพื่อ timing ตรงกับ button press
  private void PlayCastSfx(ISkillDefinition skill, GameObject owner)
  {
    if (_audio == null) return;
    var key = skill.CastSfx;
    if (key == null) return;
    _audio.PlaySFX(key, owner.transform);
  }
}
