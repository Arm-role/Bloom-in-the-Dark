using System.Threading.Tasks;
using UnityEngine;

public class SkillActionPerformer : IActionPerformer
{
    private readonly SkillController _skillController;

    public SkillActionPerformer(
        SkillController skillController)
    {
        _skillController = skillController;
    }

    public bool CanExecute(
        InteractionIntent intent,
        TargetResult target)
    {
        return target.Extra is Vector2 || target.Extra == null;
    }

    public async Task<InteractionResult> Execute(
        InteractionIntent intent,
        TargetResult target)
    {
        var targetPos = (target.Extra != null) ? 
            (Vector2)target.Extra : 
            target.Origin;

        var item = intent.SourceItem;
        
        if (target.Direction != Vector2.zero)
            _skillController.ActiveSkill(
                item,
                intent,
                item.Data.Skill,
                targetPos,
                target.Direction);
        else
            _skillController.ActiveSkill(
                item,
                intent,
                item.Data.Skill,
                targetPos);

        return InteractionResult.Consumed(null, null, ETargetType.Enemy);
    }
}