using System.Threading.Tasks;
using UnityEngine;

public class AreaCircleActionPerformer : IActionPerformer
{
    private readonly SkillSpawnController _skillController;

    public AreaCircleActionPerformer(
        SkillSpawnController skillController)
    {
        _skillController = skillController;
    }

    public bool CanExecute(
        InteractionHandleContext ctx,
        TargetResult target)
    {
        return
            ctx.ItemInstance is PlantItemInstance &&
            target.Extra is Vector2;
    }

    public async Task<InteractionResult> Execute(
        InteractionHandleContext ctx,
        TargetResult target)
    {
        Debug.Log("AreaCircleActionPerformer");
        
        var center = (Vector2)target.Extra;

        _skillController.ActiveSkill(
            ctx.ItemInstance,
            ctx,
            ctx.ItemInstance.GetProperty<string>(EItemProperty.SkillName),
            center);

        return InteractionResult.Consumed(null);
    }
}