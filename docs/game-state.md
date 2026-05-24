# Game Loop / State Machine

โครงหลักของเกม — state machine + game loop ที่ tick ทุก `IGameSystem`

## Entry point

`GameBootstrap.cs` → `Initialize()` / `StartGame()` ; `GameStateMachine.ChangeState()`

## Files

| symbol / class | path | หน้าที่ |
|----------------|------|---------|
| `GameBootstrap` | `3_Application/GameState/GameBootstrap.cs` | MonoBehaviour — สร้าง `GameApplication` + ส่ง Update (DontDestroyOnLoad) |
| `GameApplication` | `3_Application/GameState/GameApplication.cs` | ถือ `GameStateMachine` + forward Update |
| `GameStateMachine` | `3_Application/GameState/GameStateMachine.cs` | transition + notify listener |
| `GameState` (+ `GamePlayState`/`InventoryState`/`UpgradeState`/`PauseState`) | `3_Application/GameState/GameState.cs` | แต่ละ state ถือ `GameLoop` ของตัวเอง |
| `GameLoop` | `3_Application/GameState/GameLoop.cs` | รัน `IGameSystem` ที่ลงทะเบียน |
| `EGameState` | `3_Application/GameState/EGameState.cs` | enum |

## Flow

```
GameBootstrap.Initialize(container)
  → BuildApplication: สร้าง 4 state (Upgrade/GamePlay/Inventory/Pause)
                      → GameStateMachine(states) → container.Register ทุกตัว

GameBootstrap.StartGame → GameApplication.Start → stateMachine.ChangeState(Gameplay)

GameBootstrap.Update(dt)
  → GameApplication.Update → GameStateMachine.Update
      → _current.Update(dt) → GameLoop.Update → IGameSystem.Update ทุกตัว

ChangeState(type)
  → _current.Exit() → GameLoop.Exit (IGameSystem.Exit)
  → _current = states[type] → Enter() → GameLoop.Enter
  → notify ทุก IGameStateListener.OnGameStateChanged(type)
```

State transition พิเศษ (`GameApplication`): เปิด upgrade popup → `timeScale=0` + `ChangeState(Upgrade)` ; ปิด → `timeScale=1` + `Gameplay` ; `ToggleInventory` สลับ Gameplay ↔ Inventory (block ตอน `GameSession.IsPlayerRespawning`)

## Contracts (public API — เปลี่ยนช้า เชื่อถือได้)

`GameBootstrap`: `Initialize(DIContainerBase)`, `StartGame()`
`GameStateMachine`: `CurrentState`, `ChangeState(EGameState)`, `AddStateListener(IGameStateListener)`, `ResetForNewScene()`
`GameState`: `AddSystem(IGameSystem)`, `ClearSystems()`, `Enter/Exit/Update/FixedUpdate`
`GameLoop`: `AddSystem(IGameSystem)`, `Clear()`, `Enter/Exit/Update/FixedUpdate`

## Gotchas

- `IGameStateListener` ≠ `IGameSystem` — listener รับ event เปลี่ยน state, system รับ Update tick
- `GameLoop.AddSystem` ต้องเรียกตอน install (`6_Installs`) **ก่อน** `StartGame`
- `GameBootstrap` เป็น **DontDestroyOnLoad** → `Update` ยังรันตอนอยู่ scene อื่น — `HandleSceneUnloaded` เรียก `ResetForNewScene()` ล้าง system/listener กัน tick destroyed MonoBehaviour
- `ResetForNewScene` ตั้ง `_current=null` **โดยไม่เรียก `Exit()`** (system เก่า destroyed แล้ว) — ให้ `ChangeState(Gameplay)` รอบใหม่ Enter ได้จริง
- `BuildApplication` สร้าง 4 state (Upgrade/GamePlay/Inventory/Pause) — `EGameState.Trade` มีใน enum แต่ยังไม่มี state class แยก
- ทุก state ถือ `GameLoop` แยกของตัวเอง — system ที่ลงใน Gameplay ไม่ tick ตอน Inventory

## Related

- `docs/player.md` — `PlayerController` implement `IGameStateListener` (`isGamePlayStat`)
