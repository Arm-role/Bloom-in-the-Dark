#nullable enable

using System;
using System.Threading.Tasks;
using UnityEngine;

// Wandering trader event NPC — spawn ทุก N วัน ที่ Prepare phase
// Schedule:
//   Day == firstVisitDay หรือ (day - firstVisitDay) % intervalDays == 0 → spawn
//   Day == spawnedDay + stayCycles → start departure
//
// Spawn placement (Q5+Q6 — random ในรัศมีรอบ BaseBuilding, นอก camera + ไม่ติด obstacle):
//   - random direction + distance ใน [minRadius, maxRadius]
//   - skip ถ้า candidate อยู่ใน camera viewport + margin
//   - skip ถ้า OverlapCircle จับ obstacle
//   - retry maxSpawnAttempts ครั้ง — ถ้าไม่เจอ → skip visit รอบนี้
//
// Movement:
//   Spawn: walk เข้าหา BaseBuilding (Q2=B), หยุดก่อนถึง approachDistance
//   Despawn: walk ออกไป random valid position (Q7=A), ถึงปุ่ป despawn
public sealed class WanderingTraderController : IGameSystem, IDisposable
{
  private readonly ITurnSystem _turnSystem;
  private readonly IEntitySpawner _spawner;
  private readonly SpawnerHandle _spawnerHandle;
  private readonly Transform _baseTransform;
  private readonly IWanderingTraderConfig _config;
  private readonly Camera _camera;

  private NpcController? _activeTrader;
  private int _spawnedDay;
  private bool _isDeparting;
  private Vector3 _exitPosition;
  private bool _disposed;

  public WanderingTraderController(
    ITurnSystem turnSystem,
    IEntitySpawner spawner,
    SpawnerHandle spawnerHandle,
    Transform baseTransform,
    IWanderingTraderConfig config,
    Camera camera)
  {
    _turnSystem = turnSystem;
    _spawner = spawner;
    _spawnerHandle = spawnerHandle;
    _baseTransform = baseTransform;
    _config = config;
    _camera = camera;

    _turnSystem.OnNextTurn += HandleTurnChanged;
  }

  // ==========================
  // IGameSystem — Update tick สำหรับ check ว่า trader ถึง exit หรือยัง
  // ==========================
  public void Enter() { }
  public void Exit() { }
  public void FixedUpdate(float dt) { }

  public void Update(float dt)
  {
    if (!_isDeparting || _activeTrader == null) return;

    float dist = Vector3.Distance(_activeTrader.transform.position, _exitPosition);
    if (dist <= _config.ArrivalDistance)
      DespawnTrader();
  }

  public void Dispose()
  {
    if (_disposed) return;
    _turnSystem.OnNextTurn -= HandleTurnChanged;
    _disposed = true;
  }

  // ==========================
  // Schedule logic
  // ==========================

  private void HandleTurnChanged(ETurnState state)
  {
    if (state != ETurnState.Preparation) return;

    int day = _turnSystem.CurrentDay;

    // มี trader อยู่ → check departure schedule
    if (_activeTrader != null)
    {
      if (!_isDeparting && day - _spawnedDay >= _config.StayCycles)
        StartDeparture();
      return;
    }

    // ไม่มี trader → check arrival schedule
    if (ShouldSpawnToday(day))
      _ = SpawnAsync(day);
  }

  private bool ShouldSpawnToday(int day)
  {
    if (day < _config.FirstVisitDay) return false;
    return (day - _config.FirstVisitDay) % _config.IntervalDays == 0;
  }

  // ==========================
  // Spawn + walk-to-base
  // ==========================

  private async Task SpawnAsync(int day)
  {
    try
    {
      if (!TryFindValidPosition(out var pos))
      {
#if UNITY_EDITOR
        Debug.LogWarning($"[WanderingTrader] No valid spawn position at Day {day} — skip visit");
#endif
        return;
      }

      var npc = await _spawner.SpawnNpc(_config.NpcId, pos);
      if (npc == null) return;

      _activeTrader = npc;
      _spawnedDay = day;
      _isDeparting = false;

#if UNITY_EDITOR
      Debug.Log($"[WanderingTrader] Spawned at Day {day}, pos={pos}");
#endif

      // Q2=B: walk เข้าหา BaseBuilding — หยุดก่อนถึง approachDistance
      Vector3 basePos = _baseTransform.position;
      Vector3 approachDir = (basePos - pos).normalized;
      Vector3 approachTarget = basePos - approachDir * _config.ApproachDistance;
      npc.WalkToThenPatrol(approachTarget);
    }
    catch (Exception e)
    {
      Debug.LogError($"[WanderingTrader] SpawnAsync failed: {e}");
    }
  }

  // ==========================
  // Departure + walk-to-exit
  // ==========================

  private void StartDeparture()
  {
    if (_activeTrader == null || _isDeparting) return;

    if (!TryFindValidPosition(out var exitPos))
    {
      // หา exit ไม่เจอ → despawn ทันที (fallback)
#if UNITY_EDITOR
      Debug.LogWarning("[WanderingTrader] No valid exit position — despawn immediately");
#endif
      DespawnTrader();
      return;
    }

    _exitPosition = exitPos;
    _isDeparting = true;
    _activeTrader.WalkToThenPatrol(exitPos);

#if UNITY_EDITOR
    Debug.Log($"[WanderingTrader] Departing at Day {_turnSystem.CurrentDay}, exitPos={exitPos}");
#endif
  }

  private void DespawnTrader()
  {
    if (_activeTrader == null) return;
    _spawnerHandle.Despawn(_activeTrader.gameObject);
    _activeTrader = null;
    _isDeparting = false;
  }

  // ==========================
  // Random valid position (spawn + exit ใช้ logic เดียวกัน)
  // ==========================

  // Q5+Q6: random ในรัศมี [min, max] รอบ BaseBuilding, นอก camera viewport (+ margin), ไม่ติด obstacle
  // ครบ maxSpawnAttempts ยังไม่เจอ → return false
  private bool TryFindValidPosition(out Vector3 pos)
  {
    pos = default;
    Vector3 basePos = _baseTransform.position;
    float marginHalf = (_config.CameraMargin - 1f) * 0.5f;

    for (int attempt = 0; attempt < _config.MaxSpawnAttempts; attempt++)
    {
      // Random direction + distance ในช่วง [minRadius, maxRadius]
      float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
      float dist = UnityEngine.Random.Range(_config.MinRadius, _config.MaxRadius);
      Vector3 candidate = basePos + new Vector3(Mathf.Cos(angle) * dist, Mathf.Sin(angle) * dist, 0f);

      // Camera viewport check (+ margin) — skip ถ้าอยู่ในกล้อง
      Vector3 vp = _camera.WorldToViewportPoint(candidate);
      bool inViewWithMargin =
        vp.z > 0f &&
        vp.x >= -marginHalf && vp.x <= 1f + marginHalf &&
        vp.y >= -marginHalf && vp.y <= 1f + marginHalf;
      if (inViewWithMargin) continue;

      // Obstacle check — skip ถ้าติดผนัง
      var hit = Physics2D.OverlapCircle(candidate, _config.SpawnClearRadius, _config.ObstacleMask);
      if (hit != null) continue;

      pos = candidate;
      return true;
    }

    return false;
  }
}
