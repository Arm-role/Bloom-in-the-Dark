using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyTargetSelector : MonoBehaviour
{
    public string CurrentKey { get; private set; } = "AttackPlayer";

    public Transform player;
    public float preferPlayerDistance = 12f;

    private EnemyController _c;

    private void Awake() { _c = GetComponent<EnemyController>(); }

    public void EvaluateTarget()
    {
        if (player == null) player = _c.Player;

        if (player != null)
        {
            float d = Vector2.Distance(transform.position, player.position);
            if (d <= preferPlayerDistance)
            {
                CurrentKey = "AttackPlayer";
                return;
            }
        }

        CurrentKey = "AttackBase";
    }
}