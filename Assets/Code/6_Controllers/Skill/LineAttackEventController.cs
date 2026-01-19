using System;
using UnityEngine;

public class LineAttackEventController :
    MonoBehaviour,
    ISkillController,
    IPoolable<GameObject>,
    IDestructible
{
    public LineMeleeSkill Skill;
    public float TriggerTime = 0.15f; // melee speedเร็วกว่า plant

    private float timer;
    private bool isInitial;
    private Vector2 _playerPosition;

    public bool IsAlive { get; set; }

    public void Initialze(IItemInstance itemInstance, InteractionHandleContext ctx)
    {
        Skill = new LineMeleeSkill();
        Skill.Range = itemInstance.GetStat(EItemStatType.Range);
        Skill.Width = itemInstance.GetStat(EItemStatType.AreaRadius);
        Skill.Damage = itemInstance.GetStat(EItemStatType.Damage);
        Skill.KnockForce = itemInstance.GetStat(EItemStatType.KnockForce);
        Skill.KnockDoraction = itemInstance.GetStat(EItemStatType.KnockDuration);

        Skill.Direction = (ctx.PointerPosition.Value - ctx.PlayerPosition.Value).normalized;

        _playerPosition = ctx.PlayerPosition.Value;
        isInitial = true;
        timer = 0;
    }

    public void OnReturnToPool(GameObject ob)
    {
        isInitial = false;
        timer = 0;
    }

    public void OnSpawnFromPool(GameObject ob)
    {
    }

    void Update()
    {
        if (!isInitial) return;

        timer += Time.deltaTime;

        if (timer >= TriggerTime)
        {
            Skill.Cast(_playerPosition);
            RequestDestruction();
        }
    }

    private void OnDrawGizmos()
    {
        if (Skill == null)
            return;

        Vector2 origin = transform.position;
        Vector2 dir = Skill.Direction.normalized;

        Vector2 center = origin + dir * (Skill.Range * 0.5f);

        Gizmos.color = Color.red;

        Matrix4x4 old = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(
            center,
            Quaternion.Euler(0, 0, Skill.Angle),
            Vector3.one);

        Gizmos.DrawWireCube(Vector3.zero, Skill.Size);
        Gizmos.matrix = old;

        // ---- visualize origin → attack ----
        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            origin,
            origin + dir * Skill.Range
        );

        Gizmos.DrawSphere(origin, 0.05f);
    }

    public event Action<GameObject> OnRequestDestruction;

    public void RequestDestruction()
    {
        OnRequestDestruction?.Invoke(gameObject);
    }
}