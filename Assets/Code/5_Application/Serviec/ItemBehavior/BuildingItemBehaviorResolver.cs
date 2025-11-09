using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingItemBehaviorResolver", menuName = "Game/Interaction/Resolvers/BuildingItemBehaviorResolver")]
public class BuildingItemBehaviorResolver : ItemBehaviorTypeResolver
{
    private readonly Dictionary<string, IItemBehavior> _actions = new()
    {
        { "Fence", new PlacementAction() },
        { "Tuner", new PlacementAction() },
    };

    public override IItemBehavior Resolve(string itemName, InteractionTargetContext target)
    {
        if (_actions.TryGetValue(itemName, out var action))
        {
            return action;
        }

        Debug.LogWarning($"⚠️ [BuildingResolver] No placement behavior for {itemName}");
        return null;
    }
}