using System.Collections.Generic;
using UnityEngine;

public class InteractionPriorityResolver
{
    public InteractionTargetContext? ResolveBest(
         List<InteractionTargetContext> targets,
         IPriorityRule rule)
    {
        InteractionTargetContext? best = null;
        float bestScore = float.MinValue;

        foreach (var t in targets)
        {
            if (!t.IsValid) continue;

            if (t.IsObject)
            {
                var worldInteraction = t.WorldInteractable;
                float basePriority = worldInteraction.InteractionPriority;

                float mod = rule?.Evaluate(worldInteraction.Type) ?? 0;

                float score = basePriority + mod;

                if (score > bestScore)
                {
                    bestScore = score;
                    best = t;
                }
            }
            else
            if (t.IsTile)
            {
                var worldInteraction = t.TileState.WorldInteractable;
                float basePriority = worldInteraction.InteractionPriority;

                float mod = rule?.Evaluate(worldInteraction.Type) ?? 0;

                float score = basePriority + mod;

                if (score > bestScore)
                {
                    bestScore = score;
                    best = t;
                }
            }
        }

        Debug.Log(bestScore);
        return best;
    }
}