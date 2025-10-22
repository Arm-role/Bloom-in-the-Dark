using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ToolItemBehaviorResolver", menuName = "Game/Interaction/Resolvers/ToolItemBehaviorResolver")]
public class ToolItemBehaviorResolver : ItemBehaviorTypeResolver
{
    public readonly Dictionary<(string, string), IItemBehavior> Actions = new()
    {
        {("Hoe",""), new HoeAction() },
        {("Pickaxe",""), new PickaxeAction() }
    };
    public override IItemBehavior Resolve(string itemName, string tag = "")
    {
        if (Actions.TryGetValue((itemName, tag), out IItemBehavior action))
        {
            return action;
        }
        Debug.LogWarning("Not Found ToolItemBehavior");
        return null;
    }
}
