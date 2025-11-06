using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ToolItemBehaviorResolver", menuName = "Game/Interaction/Resolvers/ToolItemBehaviorResolver")]
public class ToolItemBehaviorResolver : ItemBehaviorTypeResolver
{
    private readonly Dictionary<string, IItemBehavior> _actionEmpty = new()
    {
        { "Hoe", new GridTargetAction() },
        { "Pickaxe", new GridTargetAction() },
        { "Axe", new GridTargetAction() }
    };

    private readonly Dictionary<string, IItemBehavior> _actionTile = new()
    {
        { "Hoe", new GridTargetAction() },
        { "Pickaxe", new GridTargetAction() },
        { "Axe", new GridTargetAction() }
    };

    private readonly Dictionary<string, IItemBehavior> _actionObject = new()
    {
        { "Hoe", new GridTargetAction() },
        { "Pickaxe", new GridTargetAction() },
        { "Axe", new GridTargetAction() }
    };

    public override IItemBehavior Resolve(string itemName, InteractionTarget target)
    {
        if (target.IsTile)
        {
            if (_actionTile.TryGetValue(itemName, out var actionTile))
            {
                return actionTile;
            }
        }

        if (target.IsObject)
        {
            if (_actionObject.TryGetValue(itemName, out var actionObject))
            {
                return actionObject;
            }
        }

        if (_actionEmpty.TryGetValue(itemName, out var action))
        {
            return action;
        }

        Debug.LogWarning($"⚠️ [ToolResolver] No behavior for tool {itemName}");
        return null;
    }
}
