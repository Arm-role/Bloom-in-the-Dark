using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/Cost Profile")]
public class InteractionCostService : ScriptableObject
{
    [SerializeField] private List<IntentCostEntry> costs;

    public bool TryResolve(
        EInteractionIntentType intent,
        InteractionResult result,
        out InteractionFeedback feedback)
    {
        feedback = InteractionFeedback.None(intent);

        if (result.Outcome != InteractionOutcome.Consumed)
            return false;
        
        Debug.Log("Consumed");

        foreach (var entry in costs)
        {
            if (entry.Intent != intent)
                continue;

            Debug.Log("feedback");
            feedback = entry.ToFeedback(result.Outcome);
            return true;
        }

        return false;
    }
}