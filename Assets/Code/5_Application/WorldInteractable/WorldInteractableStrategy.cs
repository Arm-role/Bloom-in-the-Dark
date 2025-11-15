using System.Threading.Tasks;
using UnityEngine;

public abstract class WorldInteractableStrategy : ScriptableObject
{
    public virtual float Priority => 10f;

    public virtual EWorldInteractableType Type { get; protected set; }

    public abstract bool CanInteract(InteractionHandleContext context);

    public abstract Task<bool> TryInteract(InteractionHandleContext context);
}
