using UnityEngine;

public class DeadState : IEnemyState
{
    private EnemyController _c;

    public DeadState(EnemyController c)
    {
        _c = c;
    }

    public void Enter()
    {
        _c.Locomotion.StopMovement();
        _c.OnRequestDisableCollision();
        _c.StartCoroutine(DestroyAfter());
    }

    public void Exit() { }

    public void ManualUpdate() { }
    public void ManualFixedUpdate() { }

    private System.Collections.IEnumerator DestroyAfter()
    {
        yield return new WaitForSeconds(1.5f);
        _c.RequestDestruction();
    }
}
