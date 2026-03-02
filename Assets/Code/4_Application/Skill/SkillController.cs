using UnityEngine;

public class SkillController
{
  private readonly SpawnerHandle _spawner;
  private readonly SkillSpawnController _skillSpawn;
  private readonly SkillSelfController _skillSelf;

  public SkillController(SpawnerHandle spawner, PlayerInteractor playerInteractor)
  {
    _spawner = spawner;
    _skillSpawn = new SkillSpawnController(_spawner);
    _skillSelf = new SkillSelfController(playerInteractor);
  }

  public void ActiveSelfSkill(
   ISkillDataPayload payload, InteractionIntent intent)
    => _skillSelf.Use(payload, intent);
  
  public void ActiveSkill(
    ISkillDataPayload payload, InteractionIntent intent, SkillDefinition skillDefinition, Vector2 targetPos)
    => _skillSpawn.ActiveSkill(payload, intent, skillDefinition, targetPos);

  public void ActiveSkill(
    ISkillDataPayload payload, InteractionIntent intent, SkillDefinition skillDefinition, Vector2 targetPos, Vector2 direction)
    => _skillSpawn.ActiveSkill(payload, intent, skillDefinition, targetPos, direction);
}