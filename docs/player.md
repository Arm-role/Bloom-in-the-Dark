# Player + Respawn System

MonoBehaviour หลักของ player + flow การตาย/เกิดใหม่

## Entry point

`PlayerController.cs` → `TakeDamage()` → `OnDied()` — จุดเริ่ม trace death flow

## Files

| symbol / class | path | หน้าที่ |
|----------------|------|---------|
| `PlayerController` | `3_Application/Player/PlayerController.cs` | MonoBehaviour หลัก — damage, energy, death, lifecycle |
| `PlayerRespawnController` | `3_Application/Player/PlayerRespawnController.cs` | นับถอยหลัง respawn + คืน state |
| `PlayerInteractor` | `3_Application/Charactor/PlayerInteractor.cs` | exec command (energy, item, cooldown) |
| `PlayerAnimation` | `5_Views/Charactor/PlayerAnimation.cs` | view (ดู `animation.md`) |

`PlayerController` implements: `IGameStateListener`, `ICombatEntity`, `IDamageable`, `IEnergyable`, `IHealthable`, `IDestructible`, `IPoolable<GameObject>`

## Flow — Death

```
TakeDamage(ctx)
  → guard !_playerHealth.IsAlive → return true   (กันตีศพระหว่าง respawn)
  → Interactor.TryExecute(TakeDamageCommand)
  → OnDamaged?.Invoke(result)  → AnimationSystem.HandleDamage → เล่น Death clip
  → if isDead → OnDied()
       • OnPlayerDied?.Invoke()              → PlayerRespawnController.HandlePlayerDied
       • UpdateMoveDirection(zero) + rb.velocity=zero + LockAnimation()
       • RaiseFinished += HandleDeathAnimationFinished

[Death clip จบ → Animation_Finished → RaiseFinished]
  → HandleDeathAnimationFinished()
       • HideVisual()
       • coroutine DeactivateAfterDeath() → wait _deathHideDelay → SetActive(false)
```

## Flow — Respawn

```
OnPlayerDied → PlayerRespawnController.HandlePlayerDied → RespawnFlow() coroutine
  • IsRespawning=true, GameSession.IsPlayerRespawning=true
  • Interactor.SetExclusiveLock("respawn")
  • camera → FreePan, view.Show()
  • นับถอยหลัง _respawnDuration (default 5s)
  • Respawn() → PlayerController.Respawn(pos, hpPercent)
       SetActive(true) → ShowVisual() → AnimationSystem.Reset() (ปลด lock)
       → health.Fill() → หัก HP ตาม hpPercent
  • camera → Follow, view.Hide(), Interactor.ReleaseLock()
```

## Contracts (public API — เปลี่ยนช้า เชื่อถือได้)

`PlayerController`:
- events: `OnDamaged`, `OnEnergy`, `OnHeal`, `OnRequestDestruction`, `OnPlayerDied`
- `IsAlive`, `Interactor`
- `TakeDamage(DamageContext) : bool`, `Heal(HealthContext)`, `AddEnergy`, `EnergyFill`
- `Respawn(Vector3, float hpPercent)` — เรียกโดย `PlayerRespawnController`
- `[SerializeField] _deathHideDelay` — delay หลัง death anim จบ ก่อน `SetActive(false)`

`PlayerRespawnController`:
- events: `OnRespawnStarted`, `OnRespawnCountdown(float)`, `OnRespawnCompleted`
- `IsRespawning`, `CancelRespawn()` (เรียกตอน GameOver)

## Gotchas

- `Update()`/`FixedUpdate()` gate ด้วย `isGamePlayStat` (Gameplay\|Inventory) **และ** `_playerHealth.IsAlive` — ตายแล้วต้องไม่ป้อน input ทับ death animation
- Player ใช้ `OnPlayerDied` **แทน** `RequestDestruction` — respawn system รับช่วงต่อ ไม่คืน pool (ต่างจาก Enemy)
- `OnDisable` ตัด subscription animation, `OnEnable` subscribe ใหม่ — รองรับ `SetActive` toggle ตอน respawn
- `ShowVisual()` ใน `Respawn()` **จำเป็น** — `HandleDeathAnimationFinished` ปิด renderer ผ่าน `HideVisual()` ถ้าไม่เปิดกลับ player respawn มาแบบล่องหน
- death flow เลียนแบบ `Enemy.DeadState`: stop movement → `LockAnimation` → รอ `RaiseFinished` → `HideVisual` → delay → ปิดตัว

## SFX (Phase 2.1)

`PlayerAudioBinder` (IDisposable ใน `3_Application/Player/`) subscribe combat events ของ `PlayerController` แล้วยิง 3D SFX ที่ player transform

| Trigger | Config field | Player event |
|---------|--------------|--------------|
| Player damage taken | `CombatSoundConfig.OnPlayerHit` | `PlayerController.OnDamaged` |
| Player death | `CombatSoundConfig.OnPlayerDeath` | `PlayerController.OnPlayerDied` |
| Player heal | `CombatSoundConfig.OnPlayerHeal` | `PlayerController.OnHeal` |

- ทุก field ใน `CombatSoundConfig` (`4_Infrastructure/Audio/`) เป็น optional → null = silent
- Binder register ใน DI container เพื่อ keep alive ตลอด scene — ถ้าไม่ register reference หายแล้ว GC เก็บ → event leak
- เสียงเล่นที่ `_player.transform` (follow target) — เป็น 3D ถ้า `SoundData.Is3D = true`
- Enemy hit/death อยู่ใน config (Phase 2.2) แต่ยังไม่มี binder — รอ iteration ถัดไป

## Related

- `animation.md` — `RaiseFinished` / `LockAnimation` / `HideVisual`
- `docs/enemy.md` — `DeadState` คือต้นแบบของ death flow นี้
- `docs/game-state.md` — `OnGameStateChanged` ตั้ง `isGamePlayStat`
- `docs/audio.md` — `IAudioService` + `ICombatSoundConfig`
