using UnityEngine;

// State ที่ enemy เข้ามาเมื่อกำแพงขวางทาง
// - ตี BreakableWall จน destroyed หรือจน timeout
// - ออกจาก state นี้โดยอัตโนมัติเมื่อกำแพงหาย → ChaseState
public class WallBreakState : IEnemyState
{
    private readonly EnemyController _c;

    private IBreakableWall _wall;

    private float _timer;
    private const float TIMEOUT = 8f;

    private Transform _cachedWallTransform;
    private float _cachedWallRadius;

    public WallBreakState(EnemyController c)
    {
        _c = c;
    }

    public void SetWall(IBreakableWall wall)
    {
        _wall = wall;
        var combat = wall as ICombatEntity;
        _cachedWallTransform = combat?.Transform;
        _cachedWallRadius = combat?.CombatRadius ?? 0.5f;
    }

    public void Enter()
    {
        _timer = TIMEOUT;
    }

    public void Exit()
    {
        _wall = null;
        _cachedWallTransform = null;
    }

    public void ManualUpdate()
    {
        _timer -= Time.deltaTime;

        if (_wall == null || _wall.IsDestroyed || _timer <= 0f)
        {
            _c.ChangeState(_c.ChaseState);
            return;
        }

        float dist = CombatDistanceUtility.EdgeDistance(
            _c.transform, _c.CombatRadius,
            _cachedWallTransform, _cachedWallRadius);

        if (_c.Combat.AnySkillReadyInRange(dist) && !_c.PatternBrain.IsRunning)
            _c.PatternBrain.Tick(_c);
    }

    public void ManualFixedUpdate() { }
}
