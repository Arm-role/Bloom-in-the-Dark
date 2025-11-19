using System;
using UnityEngine;
using System.Threading.Tasks;

[RequireComponent(typeof(Collider2D))]
public class WorldInteractable : MonoBehaviour, IWorldInteractable, IPoolable<GameObject>
{
    [SerializeField] private WorldInteractableStrategy _strategy;

    public EWorldInteractableType Type => _strategy.Type;
    public WorldInteractableStrategy Strategy => _strategy;
    public float InteractionPriority => _strategy != null ? _strategy.Priority : 0f;
    public event Action<GameObject> OnRequestDestruction;

    private WorldInteractionExecutor _executor;

    private bool _isAlive;
    public bool IsAlive => _isAlive;

    public void Init(WorldInteractionExecutor executor)
    {
        _executor = executor;
    }
    public bool CanInteract(InteractionHandleContext ctx)
    {
        if (_strategy == null) return false;
        return _strategy.CanInteract(ctx, gameObject);
    }
    public async Task<bool> TryInteract(InteractionHandleContext ctx)
    {
        if (!CanInteract(ctx)) return false;

        var action = _strategy.Evaluate(ctx, gameObject);
        return await _executor.Execute(action, gameObject);
    }

    public void RequestDestruction()
    {
        OnRequestDestruction?.Invoke(gameObject);
    }

    public void OnSpawnFromPool(GameObject ob)
    {
        if(ob != gameObject) return;

        _isAlive = true;
        Debug.Log("OnSpawnFromPool " + _isAlive);
    }

    public void OnReturnToPool(GameObject ob)
    {
        if (ob != gameObject) return;

        _isAlive = false;
        Debug.Log("OnReturnToPool " + _isAlive);
    }
}
