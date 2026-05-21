# Enemy AI System

Enemy แบบ pool-based — state machine + combat pattern + FlowField navigation

## Entry point

`EnemyController.cs` → `OnSpawnFromPool()` (เกิด) / `ChangeState()` (เปลี่ยน state)

## Files

| symbol / class | path | หน้าที่ |
|----------------|------|---------|
| `EnemyController` | `3_Application/Charactor/Enemy/EnemyController.cs` | Entry point — ถือทุก component |
| `EntityController` | `3_Application/Charactor/Enemy/EntityController.cs` | base class (`ICombatEntity`/`IDamageable`/`IDestructible`/`IPoolable`) |
| `BossController` | `3_Application/Charactor/Enemy/BossController.cs` | สืบ `EnemyController` |
| `IdleState` / `ChaseState` / `AttackState` / `DeadState` / `WallBreakState` | `3_Application/Charactor/Enemy/State/` | implement `IEnemyState` |
| `EnemyCombat` | `3_Application/Charactor/Enemy/Combat/EnemyCombat.cs` | จัดการ skill + ยิง event |
| `EnemyPatternBrain` | `3_Application/Charactor/Enemy/Combat/EnemyPatternBrain.cs` | รัน `EnemyPattern` เป็น Coroutine |
| `AOESlamPattern` / `DashAttackPattern` | `3_Application/Charactor/Enemy/Combat/` | attack pattern |

## Flow — Lifecycle (pool-based)

```
SpawnScheduler.SpawnBatch → spawner.Spawn(hash, pos)
  → EnemyController.OnSpawnFromPool
      → Initialize() — สร้าง NavigationAgent, AnimationSystem
      → ApplyConfig() — ใส่ค่าจาก EnemyConfig SO → ApplySkillsAndPattern()
      → ChangeState(IdleState)
      → AITickManager.Register(TickSensor 8fps) + Register(TickState 15fps)
```

## Flow — State machine

```
IdleState   → TickSensor พบ target → ChangeState(ChaseState)
ChaseState  → TickState update FlowField → FixedUpdate ขับ Locomotion
            → combat range ถึง → ChangeState(AttackState)
            → HasDirection=false นาน 1.5s → scan BreakableWall → ChangeState(WallBreakState)
AttackState → PatternBrain.Tick → RunPattern (Coroutine) → จบ → กลับ ChaseState
WallBreakState → ตี wall จน destroyed / timeout 8s → กลับ ChaseState
DeadState   → Health.IsAlive=false → ChangeState(DeadState)
              CancelAllSkills + StopMovement + DisableCollision + LockAnimation
              → รอ RaiseFinished → HideVisual → delay 1.5s → RequestDestruction (คืน pool)
```

## Contracts (public API — เปลี่ยนช้า เชื่อถือได้)

`EnemyController` (+ base `EntityController`):
- components: `Locomotion`, `Steering`, `Sensor`, `Combat`, `PatternBrain`, `NavigationAgent`, `Health`, `AnimationSystem`, `FlowFieldOwner`, `EnemyTargetSelector`
- states: `IdleState`, `ChaseState`, `AttackState`, `DeadState`, `WallBreakState`
- `CurrentTarget`, `DefaultTarget`, `State`, `Type`
- events: `OnGetLootable`, `OnDamaged`, `OnRequestDestruction`
- `AssignTarget(Transform, float threat=-1)`, `ChangeState(IEnemyState)`, `EnterWallBreak(IBreakableWall)`
- `AddSkill(IEnemySkill)`, `SetPattern(EnemyPattern)`, `ApplyDayScaling(hp, dmg)`
- `TakeDamage(DamageContext) : bool`, `GiveReward(DamageContext)`, `RequestDestruction()`
- `RequestNavigationPause(bool)`, `OnTargetLost(Transform)`, `OnRequestEnable/DisableCollision()`

## Gotchas

- `TickSensor` / `TickState` ถูก throttle ผ่าน `AITickManager` (ไม่ใช่ทุก frame) — เพื่อ performance
- navigation หยุดได้ 3 สาเหตุ: `_navigationPaused`, `_isMovementStopped`, `!NavigationAgent.HasValidFlow`
- โดน damage → register threat ×3 ให้ target ที่โจมตี
- `WallBreakState` suspend `TickState` ไม่ให้ target selector override `CurrentTarget`
- `OnSpawnFromPool` ต้อง reset state ทุกอย่าง (เช่น `EnemyCombat` ล้าง skill list) — instance ถูก reuse จาก pool
- death flow คือ **ต้นแบบ** ของ `PlayerController.OnDied` (ดู `player.md`)

## Related

- `docs/flow-field.md` — `ChaseState` ใช้ navigate
- `docs/pooling.md` — spawn/despawn lifecycle
- `docs/animation.md` — `LockAnimation` / `RaiseFinished` ใน `DeadState`
- `docs/cycle.md` — `CycleRuntime` spawn enemy + เรียก `ApplyDayScaling` / `AssignTarget`
