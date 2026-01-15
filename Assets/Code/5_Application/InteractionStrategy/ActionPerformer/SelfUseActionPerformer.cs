using System.Threading.Tasks;
using UnityEngine;

public sealed class SelfUseActionPerformer : IActionPerformer
{
    private readonly SkillSpawnController _skillController;

    public SelfUseActionPerformer(SkillSpawnController skillController)
    {
        _skillController = skillController;
    }

    public bool CanExecute(
        InteractionHandleContext ctx,
        TargetResult target)
    {
        return ctx.ItemInstance != null && target.IsValid;
    }

    public async Task<InteractionResult> Execute(
        InteractionHandleContext ctx,
        TargetResult target)
    {
        Debug.Log("SelfUseActionPerformer");
        var item = ctx.ItemInstance;

        // ตัวอย่าง: ใช้ skill จาก item
        if (ctx.ItemInstance.HasProperty(EItemProperty.SkillName))
        {
            _skillController.ActiveSkill(
                ctx.ItemInstance,
                ctx,
                ctx.ItemInstance.GetProperty<string>(EItemProperty.SkillName),
                target.Origin
            );

            return InteractionResult.Consumed(null);
        }

        return InteractionResult.None;
    }
}