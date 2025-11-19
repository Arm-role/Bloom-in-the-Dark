using System.Threading.Tasks;

public class AreaCircleActionPerformer : IActionPerformer
{
    private SkillInteractionController _skillInteractionController;

    public AreaCircleActionPerformer(SkillInteractionController skillInteractionController)
    {
        _skillInteractionController = skillInteractionController;
    }

    public void Setup() { }

    public Task<bool> Execute(InteractionHandleContext context, IDataProvider data)
    {
        if (data is not AreaCircleData areaCircleData) return Task.FromResult(false);
        if (areaCircleData.ItemInstance.ItemData is not PlantItem plantItem) return Task.FromResult(false);

        if (areaCircleData.PointerPosition.Value == null) return Task.FromResult(false);

        if (areaCircleData.State != null && areaCircleData.State.Value == PlacementState.Valid)
        {
            _skillInteractionController.ActiveSkill(plantItem.SkillName, areaCircleData.PointerPosition.Value);

            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
}
