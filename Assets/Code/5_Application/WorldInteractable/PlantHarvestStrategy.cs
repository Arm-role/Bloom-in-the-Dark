using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/Plant Harvest Strategy")]
public class PlantHarvestStrategy : WorldInteractableStrategy
{
    [SerializeField] private float priority;
    [SerializeField] private EWorldInteractableType type;

    public override float Priority => priority;
    public override EWorldInteractableType Type => type;

    public override bool CanInteract(InteractionHandleContext context)
    {
        return true;
    }

    public override async Task<bool> TryInteract(InteractionHandleContext context)
    {
        Debug.Log("Harvesting plant...");
        await Task.Delay(200);
        return true;
    }
}