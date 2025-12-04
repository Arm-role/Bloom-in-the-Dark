using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IEnemyState
{
    private readonly EnemyController _c;

    // tuning
    private const float REPATH_MIN = 0.25f;
    private const float REPATH_MAX = 0.9f;
    private const float MIN_TIME_AFTER_PATH = 0.12f;
    private const float PLAYER_MOVE_TRIGGER = 0.45f;
    private const float PATH_CLOSE_THRESHOLD = 0.9f;

    private float _nextRepathTime = 0f;
    private float _lastPathTime = -99f;

    private Vector3 _lastPlayerPos;
    private Vector3Int _lastEnemyTile;
    private Vector3Int _lastPlayerTile;

    public ChaseState(EnemyController c) { _c = c; }

    public void Enter()
    {
        var nav = NavigationSystem.Instance;
        _lastPlayerPos = _c.Player != null ? _c.Player.position : Vector3.zero;
        if (nav != null)
        {
            _lastEnemyTile = nav.ToTile(_c.transform.position);
            _lastPlayerTile = _c.Player != null ? nav.ToTile(_c.Player.position) : _lastEnemyTile;
        }
        _nextRepathTime = 0f;
        _lastPathTime = Time.time - 10f;
    }

    public void Exit() { }

    public void ManualUpdate()
    {
        if (_c.Data.IsDead) { _c.ChangeState(_c.DeadState); return; }
        Transform player = _c.Player;
        if (player == null) return;

        float distToPlayer = Vector2.Distance(_c.transform.position, player.position);
        if (_c.Combat.AnySkillReadyInRange(distToPlayer))
        {
            _c.Movement.ClearPath();
            _c.ChangeState(_c.AttackState);
            return;
        }

        // If we currently follow a path, avoid immediate repath while approaching waypoint
        if (_c.Movement.HasPath)
        {
            float wpDist = Vector3.Distance(_c.transform.position, _c.Movement.PeekWaypoint());
            if (wpDist > 0.12f && wpDist < 0.8f) // in progress -> don't repath
            {
                _lastPlayerPos = player.position;
                return;
            }
            if (Time.time - _lastPathTime < MIN_TIME_AFTER_PATH)
            {
                _lastPlayerPos = player.position;
                return;
            }
        }

        if (Time.time < _nextRepathTime)
        {
            if (!_c.Movement.HasPath)
            {
                Vector3 predicted = Predict(player);
                _c.Movement.MoveTowards((predicted - _c.transform.position).normalized);
            }
            _lastPlayerPos = player.position;
            return;
        }

        var nav = NavigationSystem.Instance;
        if (nav == null)
        {
            _c.Movement.ClearPath();
            _c.Movement.MoveTowards((player.position - _c.transform.position).normalized);
            _lastPlayerPos = player.position;
            return;
        }

        Vector3Int enemyTile = nav.ToTile(_c.transform.position);
        Vector3Int playerTile = nav.ToTile(player.position);

        bool enemyMovedTile = enemyTile != _lastEnemyTile;
        bool playerMovedTile = playerTile != _lastPlayerTile;

        bool playerMovedEnough = Vector3.Distance(player.position, _lastPlayerPos) >= PLAYER_MOVE_TRIGGER;

        if (!enemyMovedTile && !playerMovedTile && !playerMovedEnough)
        {
            _lastPlayerPos = player.position;
            return;
        }

        _lastEnemyTile = enemyTile;
        _lastPlayerTile = playerTile;
        _lastPlayerPos = player.position;

        RequestPath(enemyTile, playerTile);

        // adaptive cooldown
        _nextRepathTime = Time.time + Mathf.Lerp(REPATH_MIN, REPATH_MAX, Mathf.InverseLerp(1f, 12f, distToPlayer));
    }

    public void ManualFixedUpdate() { /* Movement.FollowPath executed centrally in controller */ }

    private Vector3 Predict(Transform player)
    {
        Vector3 vel = (player.position - _lastPlayerPos) / Mathf.Max(0.001f, Time.deltaTime);
        vel = Vector3.ClampMagnitude(vel, 6f);
        return player.position + vel * 0.45f;
    }

    private void RequestPath(Vector3Int enemyTile, Vector3Int playerTile)
    {
        var nav = NavigationSystem.Instance;
        if (nav == null) return;

        if (enemyTile == playerTile)
        {
            _c.Movement.ClearPath();
            _c.Movement.MoveTowards((_c.Player.position - _c.transform.position).normalized);
            _lastPathTime = Time.time;
            return;
        }

        var worldPath = nav.FindPathWorld(enemyTile, playerTile);
        if (worldPath != null && worldPath.Count > 0)
        {
            _c.Movement.SetPath(worldPath);
        }
        else
        {
            _c.Movement.ClearPath();
            _c.Movement.MoveTowards((_c.Player.position - _c.transform.position).normalized);
        }

        _lastPathTime = Time.time;
    }
}