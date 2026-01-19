using UnityEngine;

public class SkillSpawnController
{
    private readonly SpawnerHandle _spawner;

    public SkillSpawnController(SpawnerHandle spawner)
    {
        _spawner = spawner;
    }

    public async void ActiveSkill(
        IItemInstance item,
        InteractionHandleContext context,
        string skillName,
        Vector2 targetPos)
    {
        if (skillName == string.Empty) return;

        GameObject ob = await _spawner.SpawnAsync(skillName, targetPos);
        var itemController = ob.GetComponent<ISkillController>();
        itemController.Initialze(item, context);
    }

    public async void ActiveSkill(
        IItemInstance item,
        InteractionHandleContext context,
        string skillName,
        Vector2 targetPos,
        Vector2 direction)
    {
        if(skillName == string.Empty) return;

        GameObject ob = await _spawner.SpawnAsync(skillName, targetPos, direction);
        var itemController = ob.GetComponent<ISkillController>();
        itemController.Initialze(item, context);
    }
}