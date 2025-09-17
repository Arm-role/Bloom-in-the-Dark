using UnityEngine;

public abstract class PrimaryActionTypeResolver : ScriptableObject
{
    public abstract IPrimaryAction Resolve(string itemName);
}
