using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantItemBehaviorResolver", menuName = "Game/Interaction/Resolvers/PlantItemBehaviorResolver")]
public class PlantItemBehaviorResolver : ItemBehaviorTypeResolver
{
    private readonly Dictionary<string, IItemBehavior> _actions = new()
    {
        {"Dragonis_Ember_Fruit", new SkillAreaCircleAction() },
        {"Cryoberry_Essence",new SkillAreaCircleAction() }
    };

    public override IItemBehavior Resolve(string itemName, InteractionTarget target)
    {
        if (_actions.TryGetValue(itemName, out var action))
        {
            return action;
        }

        Debug.LogWarning($"⚠️ [BuildingResolver] No placement behavior for {itemName}");
        return null;
    }
}
