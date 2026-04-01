using UnityEngine;

public class PlantState : MonoBehaviour
{
    public int GrowthStage { get; private set; }
    public int MaxStage;
    public bool IsGrown => GrowthStage >= MaxStage;

    public void Grow()
    {
        if (GrowthStage < MaxStage)
        {
            GrowthStage++;
        }
    }

    public void ResetStage() => GrowthStage = 0;
}