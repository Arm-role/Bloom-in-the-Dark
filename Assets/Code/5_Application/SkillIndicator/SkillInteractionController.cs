using UnityEngine;

public class SkillInteractionController
{
    private readonly GameObjectSpawner _spawner;

    public SkillInteractionController(GameObjectSpawner spawner)
    {
        _spawner = spawner;
    }

    public async void ActiveSkill(
        PlantItemInstance plantItem, 
        string skillName,
        Vector2 targetPos)
    {
        GameObject ob = await _spawner.SpawnAsync(skillName, targetPos);
        var plantController = ob.GetComponent<ISkillController<PlantItemInstance>>();
        plantController.Initialze(plantItem);
    }

    public async void ActiveSkill(
        PlantItemInstance plantItem,
        string skillName, 
        Vector2 targetPos, 
        Vector2 direction)
    {
        GameObject ob = await _spawner.SpawnAsync(skillName, targetPos, direction);
        var plantController = ob.GetComponent<ISkillController<PlantItemInstance>>();
        plantController.Initialze(plantItem);
    }
}