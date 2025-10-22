using Codice.CM.Common;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlantItemBehaviorResolver", menuName = "Game/Interaction/Resolvers/PlantItemBehaviorResolver")]
public class PlantItemBehaviorResolver : ItemBehaviorTypeResolver
{
    public readonly Dictionary<(string, string), IItemBehavior> Actions = new()
    {
        {("Musmoke",""), new MusmokeAction() }
    };
    public override IItemBehavior Resolve(string itemName, string tag = "")
    {
        if (Actions.TryGetValue((itemName, tag), out IItemBehavior action))
        {
            return action;
        }
        Debug.LogWarning("Not Found PlantItemBehavior");
        return null;
    }
}
