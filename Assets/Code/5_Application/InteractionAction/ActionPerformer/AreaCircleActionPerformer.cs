public class AreaCircleActionPerformer : IActionPerformer
{
    private SkillInteractionController _skillInteractionController;

    public AreaCircleActionPerformer(SkillInteractionController skillInteractionController)
    {
        _skillInteractionController = skillInteractionController;
    }

    public void Setup() { }

    public void Execute(IDataProvider data)
    {
        if (data is not AreaCircleData areaCircleData) return;
        if (areaCircleData.ItemInstance.ItemData is not PlantItem plantItem) return;

        if (areaCircleData.PointerPosition.Value == null) return;
        if (areaCircleData.State != null && areaCircleData.State.Value == PlacementState.Valid)
        {
            _skillInteractionController.ActiveSkill(plantItem.SkillName, areaCircleData.PointerPosition.Value);
        }
    }
}
