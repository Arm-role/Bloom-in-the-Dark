using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/Cost Profile")]
public class InteractionCostService : ScriptableObject
{
    [SerializeField] private List<IntentCostEntry> costs;

    public bool TryResolve(
        EInteractionIntentType intent,
        out InteractionFeedback feedback)
    {
        foreach (var entry in costs)
        {
            if (entry.Intent != intent)
                continue;

            feedback = entry.ToFeedback();
            return true;
        }

        feedback = InteractionFeedback.None(intent);
        return false;
    }
}