using System.Threading.Tasks;
using UnityEngine;

public class SkillActionPerformer : IActionPerformer
{
    private readonly SkillSpawnController _skillController;

    public SkillActionPerformer(
        SkillSpawnController skillController)
    {
        _skillController = skillController;
    }

    public bool CanExecute(
        InteractionHandleContext ctx,
        TargetResult target)
    {
        return target.Extra is Vector2;
    }

    public async Task<InteractionResult> Execute(
        InteractionHandleContext ctx,
        TargetResult target)
    {
        var targetPos = target.Origin;

        if (target.Direction != Vector2.zero)
            _skillController.ActiveSkill(
                ctx.ItemInstance,
                ctx,
                ctx.ItemInstance.GetProperty<string>(EItemProperty.SkillName),
                targetPos,
                target.Direction);
        else
            _skillController.ActiveSkill(
                ctx.ItemInstance,
                ctx,
                ctx.ItemInstance.GetProperty<string>(EItemProperty.SkillName),
                targetPos);

        return InteractionResult.Consumed(null);
    }
}