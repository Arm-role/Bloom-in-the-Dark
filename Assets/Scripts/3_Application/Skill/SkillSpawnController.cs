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
    GameObject owner,
    InteractionIntent intent,
    ISkillDefinition skillDefinition,
    Vector2 targetPos)
  {
    if (skillDefinition == null) return;

    var ob = await _spawner.SpawnAsync(skillDefinition.SkillId, targetPos);
    var skillExecutor = ob.GetComponent<ISkillExecutor>();
    InitialzeSkill(payload, owner, intent, skillExecutor);
  }

  public async void ActiveSkill(
    ISkillDataPayload payload,
    GameObject owner,
    InteractionIntent intent,
    ISkillDefinition skillDefinition,
    Vector2 targetPos,
    Vector2 direction)
  {
    if (skillDefinition == null) return;

    var ob = await _spawner.SpawnAsync(skillDefinition.SkillId, targetPos, direction);
    var skillExecutor = ob.GetComponent<ISkillExecutor>();
    InitialzeSkill(payload, owner, intent, skillExecutor);
  }

  private void InitialzeSkill(
    ISkillDataPayload payload,
    GameObject owner,
    InteractionIntent intent,
    ISkillExecutor skillExecutor)
  {
    Debug.Log("SkillSpawn");

    if (!skillExecutor.Initialize(
          intent.Origin.GetValueOrDefault(),
          intent.Direction.GetValueOrDefault(),
          payload,
          owner,
          intent
        ))
    {
      Debug.LogWarning($"{skillExecutor}.Initialize({payload})");
    }
  }
}