using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IEnemyState
{
    private readonly EnemyController _c;

    private float _nextRepathTime;
    private const float REPATH_MIN = 0.25f;
    private const float REPATH_MAX = 0.9f;

    private Vector3Int _lastEnemyTile;
    private Vector3Int _lastPlayerTile;

    public ChaseState(EnemyController c)
    {
        _c = c;
    }

    public void Enter()
    {
        var nav = NavigationSystem.Instance;

        if (nav != null)
        {
            _lastEnemyTile = nav.ToTile(_c.transform.position);
            _lastPlayerTile = nav.ToTile(_c.Player.position);
        }

        _nextRepathTime = 0f;
        _c.Movement.ResetStuck();
    }

    public void Exit() { }

    public void ManualUpdate()
    {
        if (_c.Data.IsDead)
        {
            _c.ChangeState(_c.DeadState);
            return;
        }

        Transform player = _c.Player;
        if (player == null) return;

        float dist = Vector2.Distance(_c.transform.position, player.position);

        // -------------------------------
        // 1) Attack check
        // -------------------------------
        if (_c.Combat.AnySkillReadyInRange(dist))
        {
            _c.Movement.ClearPath();
            _c.ChangeState(_c.AttackState);
            return;
        }

        // -------------------------------
        // 2) STUCK → IMMEDIATE REPATH
        // -------------------------------
        if (_c.Movement.StuckJustTriggered)
        {
            ImmediateRepath();
            return;
        }

        // -------------------------------
        // 3) Cooldown
        // -------------------------------
        if (Time.time < _nextRepathTime)
        {
            if (!_c.Movement.HasPath)
                _c.Movement.MoveTowards((player.position - _c.transform.position).normalized);

            return;
        }

        // -------------------------------
        // 4) Tile-change repath
        // -------------------------------
        var nav = NavigationSystem.Instance;
        if (nav == null)
        {
            _c.Movement.ClearPath();
            _c.Movement.MoveTowards((player.position - _c.transform.position).normalized);
            return;
        }

        Vector3Int enemyTile = nav.ToTile(_c.transform.position);
        Vector3Int playerTile = nav.ToTile(player.position);

        bool changedTile =
            (enemyTile != _lastEnemyTile) ||
            (playerTile != _lastPlayerTile);

        if (!changedTile)
            return;

        _lastEnemyTile = enemyTile;
        _lastPlayerTile = playerTile;

        RequestPath(enemyTile, playerTile, dist);
    }

    public void ManualFixedUpdate() { }


    // =====================================================
    // PATH LOGIC
    // =====================================================

    private void ImmediateRepath()
    {
        var nav = NavigationSystem.Instance;
        if (nav == null) return;

        Vector3Int enemyTile = nav.ToTile(_c.transform.position);
        Vector3Int playerTile = nav.ToTile(_c.Player.position);

        RequestPath(enemyTile, playerTile, 0f);
    }

    private void RequestPath(Vector3Int enemyTile, Vector3Int playerTile, float dist)
    {
        var nav = NavigationSystem.Instance;
        if (nav == null) return;

        var worldPath = nav.FindPathWorld(enemyTile, playerTile);

        if (worldPath != null && worldPath.Count > 0)
        {
            _c.Movement.SetPath(worldPath);
            _c.Movement.ResetStuck(); // IMPORTANT
        }
        else
        {
            _c.Movement.ClearPath();
            _c.Movement.MoveTowards((_c.Player.position - _c.transform.position).normalized);
        }

        // adaptive repath interval
        _nextRepathTime =
            Time.time + Mathf.Lerp(REPATH_MIN, REPATH_MAX, Mathf.InverseLerp(1f, 12f, dist));
    }
}