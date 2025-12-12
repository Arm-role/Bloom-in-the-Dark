using UnityEngine;

public class SkillInteractionController
{
    private readonly GameObjectSpawner _spawner;

    public SkillInteractionController(GameObjectSpawner spawner)
    {
        _spawner = spawner;
    }

    public async void ActiveSkill<T>(
        T item,
        InteractionHandleContext context,
        string skillName,
        Vector2 targetPos)
    {
        GameObject ob = await _spawner.SpawnAsync(skillName, targetPos);
        var itemController = ob.GetComponent<ISkillController<T>>();
        itemController.Initialze(item, context);
    }

    public async void ActiveSkill<T>(
        T item,
        InteractionHandleContext context,
        string skillName,
        Vector2 targetPos,
        Vector2 direction) where T : IItemInstance
    {
        GameObject ob = await _spawner.SpawnAsync(skillName, targetPos, direction);
        var itemController = ob.GetComponent<ISkillController<T>>();
        itemController.Initialze(item, context);
    }
}