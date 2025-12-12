using System.Threading.Tasks;
using UnityEngine;

public class LineAttackPerformer : IActionPerformer
{
    private SkillInteractionController _skillInteractionController;

    public LineAttackPerformer(SkillInteractionController skillInteractionController)
    {
        _skillInteractionController = skillInteractionController;
    }

    public void Setup() { }
    public bool CanExecute(InteractionHandleContext ctx, IDataProvider data) => true;
    public Task<bool> Execute(InteractionHandleContext ctx, IDataProvider data)
    {
        var line = (LineAttackData)data;
        var toolItem = (ToolItemInstance)ctx.ItemInstance;

        if (line.IsValid)
        {
            _skillInteractionController.ActiveSkill(
                toolItem,
                ctx,
                toolItem.ToolData.SkillName,
                line.Origin.Value,
                line.Direction.Value);

            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
}