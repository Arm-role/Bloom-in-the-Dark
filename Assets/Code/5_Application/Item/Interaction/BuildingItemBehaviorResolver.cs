using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingItemBehaviorResolver", menuName = "Game/Interaction/Resolvers/BuildingItemBehaviorResolver")]
public class BuildingItemBehaviorResolver : ItemBehaviorTypeResolver
{
    public readonly Dictionary<(string, string), IItemBehavior> Actions = new()
{
    { ("Fence", ""), new FenceAction() },
    { ("Bed", ""), new FenceAction() },
    { ("Tuner", ""), new FenceAction() },

};
    public override IItemBehavior Resolve(string itemName, string tag = "")
    {
        if (Actions.TryGetValue((itemName, tag), out IItemBehavior action))
        {
            return action;
        }
        Debug.LogWarning("Not Found BuildingItemBehavior");
        return null;
    }
}