using UnityEngine;

public abstract class DropTypeResolver : ScriptableObject
{
    public abstract IDrop Resolve(Collider2D collider);
}
