using UnityEngine;

public class SkillSpawnController
{
  private readonly SpawnerHandle _spawner;

  private IPlayerInput _input;
  private IPlayerInteractor _interactor;

  public SkillSpawnController(SpawnerHandle spawner)
  {
    _spawner = spawner;
  }

  public void SetChannelContext(IPlayerInput input, IPlayerInteractor interactor)
  {
    _input = input;
    _interactor = interactor;
  }

  public async void ActiveSkill(
    ISkillDataPayload payload,
    GameObject owner,
    InteractionIntent intent,
    ISkillDefinition skillDefinition,
    Vector2 targetPos)
  {
    if (skillDefinition == null) return;

    try
    {
      var ob = await _spawner.SpawnAsync(skillDefinition.SkillId, targetPos);
      var skillExecutor = ob.GetComponent<ISkillExecutor>();
      InitialzeSkill(payload, owner, intent, skillExecutor);
    }
    catch (System.Exception ex)
    {
      Debug.LogError($"[SkillSpawnController] ActiveSkill failed (skillId={skillDefinition.SkillId}): {ex.Message}");
    }
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

    try
    {
      var ob = await _spawner.SpawnAsync(skillDefinition.SkillId, targetPos, direction);
      var skillExecutor = ob.GetComponent<ISkillExecutor>();
      InitialzeSkill(payload, owner, intent, skillExecutor);
    }
    catch (System.Exception ex)
    {
      Debug.LogError($"[SkillSpawnController] ActiveSkill (directional) failed (skillId={skillDefinition.SkillId}): {ex.Message}");
    }
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
      return;
    }

    if (skillExecutor is IChanneledSkillExecutor channeled)
      channeled.BindChannel(_input, _interactor);
  }
}