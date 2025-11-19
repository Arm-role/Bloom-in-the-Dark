using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantItemBehaviorResolver", menuName = "Game/Interaction/Resolvers/PlantItemBehaviorResolver")]
public class PlantItemBehaviorResolver : ItemBehaviorTypeResolver
{
    private readonly Dictionary<string, IItemBehavior> _actions = new()
    {
    };

    public override IItemBehavior Resolve(string itemName, InteractionTargetContext target)
    {
        if (_actions.TryGetValue(itemName, out var action))
        {
            return action;
        }

        //Debug.LogWarning($"⚠️ [BuildingResolver] No placement behavior for {itemName}");
        return new SkillAreaCircleAction();
    }
}
