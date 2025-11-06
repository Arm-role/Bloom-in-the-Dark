using UnityEngine;

public abstract class ItemBehaviorTypeResolver : ScriptableObject
{
    public abstract IItemBehavior Resolve(string itemName, InteractionTarget target);
}
