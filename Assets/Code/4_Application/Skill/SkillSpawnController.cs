using UnityEngine;

public class SkillSpawnController
{
  private readonly SpawnerHandle _spawner;

  public SkillSpawnController(SpawnerHandle spawner)
  {
    _spawner = spawner;
  }

  public async void ActiveSkill(
    ISkillDataPayload payload,
    InteractionIntent intent,
    SkillDefinition skillDefinition,
    Vector2 targetPos)
  {
    if (skillDefinition == null) return;

    var ob = await _spawner.SpawnAsync(skillDefinition.name, targetPos);
    var skillExecutor = ob.GetComponent<ISkillExecutor>();
    InitialzeSkill(payload, intent, skillExecutor);
  }

  public async void ActiveSkill(
    ISkillDataPayload payload,
    InteractionIntent intent,
    SkillDefinition skillDefinition,
    Vector2 targetPos,
    Vector2 direction)
  {
    if (skillDefinition == null) return;

    var ob = await _spawner.SpawnAsync(skillDefinition.name, targetPos, direction);
    var skillExecutor = ob.GetComponent<ISkillExecutor>();
    InitialzeSkill(payload, intent, skillExecutor);
  }

  private void InitialzeSkill(
    ISkillDataPayload payload,
    InteractionIntent intent,
    ISkillExecutor skillExecutor)
  {
    if (!skillExecutor.Initialize(
          intent.Origin.GetValueOrDefault(),
          intent.Direction.GetValueOrDefault(),
          payload
        ))
    {
      Debug.LogWarning($"{skillExecutor}.Initialize({payload})");
    }
  }
}