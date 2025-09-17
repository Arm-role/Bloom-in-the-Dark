using UnityEngine;

public abstract class SecorndaryActionTypeResolver : ScriptableObject
{
    public abstract ISecondaryAction Resolve(string itemName);
}
