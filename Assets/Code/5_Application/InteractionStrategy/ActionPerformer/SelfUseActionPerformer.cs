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
        InteractionIntent intent,
        TargetResult target)
    {
        return intent.SourceItem != null && target.IsValid;
    }

    public async Task<InteractionResult> Execute(
        InteractionIntent intent,
        TargetResult target)
    {
        Debug.Log("SelfUseActionPerformer");
        var item = intent.SourceItem;

        // ตัวอย่าง: ใช้ skill จาก item
        if (item.HasProperty(EItemProperty.SkillName))
        {
            _skillController.ActiveSkill(
                item,
                intent,
                item.GetProperty<string>(EItemProperty.SkillName),
                target.Origin
            );

            return InteractionResult.Consumed(null);
        }

        return InteractionResult.None;
    }
}