using UnityEngine;

public class ProximityActionPerformer : IActionPerformer
{
    public void Setup() { }

    public void Execute(IDataProvider data)
    {
        if (data is not ProximityInteractionData proximityData) return;
    }
}
