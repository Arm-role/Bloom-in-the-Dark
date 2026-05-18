using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IEnemyState
{
    private readonly EnemyController _c;

    private float _repathTimer;
    private const float REPATH_INTERVAL = 0.25f;

    private Transform _cachedTarget;
    private float _cachedTargetRadius;

    // stuck detection
    private float _stuckTimer;
    private const float STUCK_THRESHOLD = 1.5f;
    private const float WALL_SCAN_DISTANCE = 2.5f;

    public ChaseState(EnemyController c)
    {
        _c = c;
    }

    public void Enter()
    {
        _repathTimer = 0f;
        _stuckTimer = 0f;
    }

    public void Exit()
    {
        _stuckTimer = 0f;
    }

    private float GetTargetRadius()
    {
        var t = _c.CurrentTarget;
        if (t != _cachedTarget)
        {
            _cachedTarget = t;
            _cachedTargetRadius = t?.GetComponent<ICombatEntity>()?.CombatRadius ?? 0.5f;
        }
        return _cachedTargetRadius;
    }

    public void ManualUpdate()
    {
        if (_c.CurrentTarget == null) { _c.ChangeState(_c.IdleState); return; }

        float ownerRadius = _c.CombatRadius;
        float targetRadius = GetTargetRadius();
        float dist = CombatDistanceUtility.EdgeDistance(
            _c.transform, ownerRadius,
            _c.CurrentTarget, targetRadius);

        if (_c.Combat.AnySkillReadyInRange(dist))
        {
            _c.ChangeState(_c.AttackState);
            return;
        }

        // ตรวจว่า enemy ไม่มีทิศทางจาก FlowField (อาจโดน wall ขวาง)
        bool hasDirection = _c.Steering.HasDirection(_c.FlowFieldOwner.Footprint);
        if (!hasDirection && !_c.NavigationAgent.HasReachedTarget())
        {
            _stuckTimer += Time.deltaTime;
            if (_stuckTimer >= STUCK_THRESHOLD)
            {
                _stuckTimer = 0f;
                TryEnterWallBreak();
            }
        }
        else
        {
            _stuckTimer = 0f;
        }

        _repathTimer -= Time.deltaTime;
        if (_repathTimer <= 0f)
        {
            _repathTimer = REPATH_INTERVAL;
            _c.NavigationAgent.RequestFlowUpdateImmediate();
        }
    }

    private void TryEnterWallBreak()
    {
        if (_c.DefaultTarget == null) return;

        Vector2 toTarget = ((Vector2)_c.DefaultTarget.position - (Vector2)_c.transform.position).normalized;

        IReadOnlyList<WorldCell> cells = FlowFieldManager.Instance.world.GetCellsAlongLine(
            _c.transform.position,
            toTarget,
            WALL_SCAN_DISTANCE);

        foreach (var cell in cells)
        {
            if (cell.Object == null) continue;
            if (!cell.Object.TryGetComponent<IBreakableWall>(out var wall)) continue;
            if (wall.IsDestroyed) continue;

            _c.EnterWallBreak(wall);
            return;
        }
    }

    public void ManualFixedUpdate()
    {
        // movement handled by steering
    }
}
