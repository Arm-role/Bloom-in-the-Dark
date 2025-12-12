using System.Threading.Tasks;

public class AreaCircleActionPerformer : IActionPerformer
{
    private SkillInteractionController _skillInteractionController;

    public AreaCircleActionPerformer(SkillInteractionController skillInteractionController)
    {
        _skillInteractionController = skillInteractionController;
    }

    public void Setup() { }
    public bool CanExecute(InteractionHandleContext ctx, IDataProvider data)
    {
        return true;
    }

    public Task<bool> Execute(InteractionHandleContext context, IDataProvider data)
    {
        var areaCircleData = (AreaCircleData)data;
        var plantItemInstance = (PlantItemInstance)context.ItemInstance;

        if (areaCircleData.State != null && areaCircleData.State.Value == PlacementState.Valid)
        {
            _skillInteractionController.ActiveSkill(
                plantItemInstance,
                context,
                plantItemInstance.PlantData.SkillName,
                areaCircleData.PointerPosition.Value);

            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
}
