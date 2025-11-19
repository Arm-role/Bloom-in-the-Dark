using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/PlantHarvestStrategy")]
public class PlantHarvestStrategy : WorldInteractableStrategy
{
    [SerializeField] private float priority;
    [SerializeField] private EWorldInteractableType type;

    public override float Priority => priority;
    public override EWorldInteractableType Type => type;

    public override bool CanInteract(InteractionHandleContext ctx, GameObject target)
    {
        var plantState = target.GetComponent<PlantState>();
        return plantState != null && plantState.IsGrown;
    }

    public override WorldAction Evaluate(InteractionHandleContext ctx, GameObject target)
    {
        var result = new WorldAction();
        var harvest = target.GetComponent<PlantHarvestHandler>();

        ItemStack[] loot;

        if (ctx.ItemInstance is ToolItem tool)
            loot = harvest.GetHarvestLoot(tool);
        else
            loot = harvest.GetHarvestLoot();

        foreach (var stack in loot)
            result.ItemRewards.Add(stack);

        result.DestroySelf = true;
        return result;
    }
}