# Bloom-in-the-Dark — Script Navigation Guide

อ่านไฟล์นี้ก่อนเสมอก่อนเปิดไฟล์ใดๆ เพื่อลด token

---

## โครงสร้าง Layer (อ่าน layer ไหน → ไปโฟลเดอร์ไหน)

| Layer | โฟลเดอร์ | ประเภท |
|-------|----------|--------|
| 1_Abstractions | Interfaces, base types | Pure C# |
| 2_Logic | Business logic, ไม่มี Unity dependency | Pure C# |
| 3_Application | MonoBehaviours, game systems | Unity |
| 4_Infrastructure | ScriptableObjects, Unity implementations | Unity SO |
| 5_Views | UI views | Unity |
| 6_Installs | DI / installer scripts | Unity |

---

## Interaction System (ระบบ click/interact กับ world)

### Flow หลัก
```
DragDropController.OnInteraction
  → ItemInteractionAction.ProcessInteractionContext
      ├─ ไม่ถือ item / capability=null → HandleGlobalInteraction → ExecuteTargetedGlobal (intent=Harvest)
      └─ ถือ item ที่มี capability → HandleInteraction
            ├─ TryGetInteractionRule ไม่เจอ rule → HandleGlobalInteraction (fallback)
            └─ เจอ rule → ExecuteTargeted → ExecuteAction
                  → CellActionPerformer.Prepare (หา targetMask)
                  → InteractionCostResolver.TryResolve (หา cost config)
                  → CommitPendingAction
                      → CellInteractionPipeline.Execute → ICellAction.Process
                      → WorldInteractionExecutor.Execute (GiveRewards, damage, tiles)
                      → ApplyFeedback (consume energy/item, apply cooldown)
```

### ไฟล์หลัก
| ไฟล์ | หน้าที่ |
|------|---------|
| `3_Application/InteractionStrategy/ItemInteractionAction.cs` | Orchestrator ทั้งหมด |
| `3_Application/Factory/CellInteractionPipeline.cs` | Resolve + Execute ICellAction |
| `3_Application/Interactable/Executor/WorldInteractionExecutor.cs` | Apply rewards, damage, tiles |
| `3_Application/Interactable/InteractionCostResolver.cs` | คำนวณ cost จาก config |
| `4_Infrastructure/Interacable/InteractionCostConfig.cs` | SO: cost entries (intent+tags+target) |
| `2_Logic/Interaction/InteractionIntentMatchRule.cs` | Match logic สำหรับ cost config |
| `3_Application/Factory/GameActionFactory.cs` | Register ICellActions ให้แต่ละ object |

### ICellAction ทั้งหมด
| Action | Stage | เงื่อนไข |
|--------|-------|----------|
| `PlaceOfferingAction` | Pre | HasItem + UsePlace tag + altar not occupied |
| `RemoveOfferingAction` | Pre | altar occupied |
| `PlantHarvestAction` | Pre | plant growth controller, harvestable |
| `RemoveSeedAction` | Pre | plant growth controller, not harvestable |
| `ClearableAction` | Pre | ClearableState |
| `PlantSeedAction` (tile) | Pre | SoilTile + HasItem + seed tag |
| `TillGrassToSoilAction` (tile) | Pre | GrassTile + tool tag |
| `RemoveSoilAction` (tile) | Pre | SoilTile + ไม่มี seed |
| `RemoveSeedAction` (tile) | Pre | SoilTile + มี seed |

---

## Inventory System

### ไฟล์หลัก
| ไฟล์ | หน้าที่ |
|------|---------|
| `3_Application/Inventory/PlayerInventory.cs` | API ระดับ player (AddItem, TryPick, Place, QuickMove) |
| `2_Logic/Inventory/InventoryLogic.cs` | Core logic (TryAddItem, TryRemoveItem, CanAddItem) |

### AddItem flow
```
PlayerInventory.AddItem(item, amount)
  1. Hotbar.TryAddItem → stack ของเดิม + fill empty slots
  2. MainInventory.TryAddItem → เหมือนกัน
  3. ถ้า remaining > 0: Hotbar.CanAddItem check แล้ว TryAddItem อีกรอบ
  4. return MainInventory.TryAddItem(remaining)
```

### ข้อควรระวัง
- `TryRemoveItem` ใน PlayerInventory เอาออกจาก **Hotbar เท่านั้น** (ไม่ touch MainInventory)
- `InventoryLogic.TryAddItem` fill existing stacks ก่อน แล้วค่อย fill empty slots

---

## Offering Altar

### ไฟล์
| ไฟล์ | หน้าที่ |
|------|---------|
| `3_Application/Progression/Upgrade/OfferingAltarController.cs` | TryPlaceItem, RemoveItem, IsOccupied |
| `3_Application/Progression/Upgrade/AltarController.cs` | OnOfferingPlaced, OnOfferingRemoved |
| `3_Application/Progression/Upgrade/AltarDomain.cs` | domain logic |
| `2_Logic/Progression/Upgrade/EAltarMode.cs` | enum |
| `3_Application/Interactable/CellAction/Object/PlaceOfferingAction.cs` | place logic |
| `3_Application/Interactable/CellAction/Object/RemoveOfferingAction.cs` | remove logic |

### Bug pattern ที่เจอ (fixed)
- Remove while holding item ไม่ทำงาน → เพราะ HandleInteraction ไม่มี global fallback
- UsePlace cost ถูก apply ตอน remove → เพราะ cost ผูกกับ intent type ไม่ใช่ action จริง

---

## Item Capability System

### ไฟล์
| ไฟล์ | หน้าที่ |
|------|---------|
| `4_Infrastructure/Item/Modules/ItemInteractionCapability.cs` | SO: list of InteractionRules |
| `2_Logic/Interacable/InteractionRule/InteractionRule.cs` | Input, Phase, Condition, Fallback, IntentType, Strategy |
| `2_Logic/Interacable/InteractionRule/InteractionCondition.cs` | Flags enum |
| `2_Logic/Interacable/InteractionRule/InteractionFallback.cs` | None / Global |

### Rule matching
`TryGetInteractionRule(input, phase, ctx)` → iterate rules → match Input + PhaseMask + Condition.IsMet(ctx)

ถ้าไม่เจอ rule → fallback to `HandleGlobalInteraction` (ตั้งแต่ commit แก้ bug แล้ว)

---

## Player Systems

| ไฟล์ | หน้าที่ |
|------|---------|
| `3_Application/Charactor/PlayerInteractor.cs` | CanExecute/TryExecute commands (energy, item, cooldown) |
| `3_Application/Interactable/InteractionRuntimeState.cs` | cooldown dictionary |
| `3_Application/Interactable/InteractionCostResolver.cs` | TryResolve + ApplyCost |

---

## Progression / Upgrade

โฟลเดอร์: `2_Logic/Progression/Upgrade/` และ `3_Application/Progression/Upgrade/`

ยังไม่ได้ explore ละเอียด — ถ้าต้องการ ดู `AltarController`, `AltarDomain` ก่อน

---

## Enemy / AI

### ไฟล์หลัก
| ไฟล์ | หน้าที่ |
|------|---------|
| `3_Application/Charactor/Enemy/EnemyController.cs` | Entry point — ถือทุก component ของ enemy |
| `3_Application/Charactor/Enemy/State/IdleState.cs` | รอ target |
| `3_Application/Charactor/Enemy/State/ChaseState.cs` | วิ่งไล่ target ผ่าน FlowField |
| `3_Application/Charactor/Enemy/State/AttackState.cs` | เข้าโจมตี |
| `3_Application/Charactor/Enemy/State/DeadState.cs` | ตาย → return to pool |
| `3_Application/Charactor/Enemy/Combat/EnemyCombat.cs` | จัดการ skill + ยิง event ไปให้ Controller |
| `3_Application/Charactor/Enemy/Combat/EnemyPatternBrain.cs` | รัน EnemyPattern เป็น Coroutine |
| `3_Application/Charactor/Enemy/Combat/AOESlamPattern.cs` | pattern: กระโดดทุบ AOE |
| `3_Application/Charactor/Enemy/Combat/DashAttackPattern.cs` | pattern: พุ่งเข้าหา |

### Lifecycle ของ Enemy (pool-based)
```
SpawnScheduler.SpawnBatch
  → _spawner.Spawn(hash, pos)
  → EnemyController.OnSpawnFromPool
      → Initialize() — สร้าง NavigationAgent, AnimationSystem
      → ApplyConfig() — ใส่ค่าจาก EnemyConfig SO
          → ApplySkillsAndPattern() — ลง skill + pattern ให้ PatternBrain
      → ChangeState(IdleState)
      → AITickManager.Register(TickSensor, 8fps) + Register(TickState, 15fps)
```

### State Transition
```
IdleState
  → TickSensor พบ target → ChangeState(ChaseState)
ChaseState
  → TickState update FlowField → FixedUpdate ขับ Locomotion
  → Combat range ถึง → ChangeState(AttackState)
  → HasDirection=false นาน 1.5s → scan BreakableWall → ChangeState(WallBreakState)
AttackState
  → PatternBrain.Tick → RunPattern (Coroutine)
  → Pattern จบ → กลับ ChaseState
WallBreakState
  → ตี BreakableWall จน IsDestroyed หรือ timeout 8s → กลับ ChaseState
  → TickState ถูก suspend ไม่ให้ target selector override CurrentTarget
DeadState
  → TakeDamage → Health.IsAlive=false → ChangeState(DeadState)
  → GiveReward → return to pool
```

### ข้อควรระวัง
- `TickSensor` และ `TickState` ถูก throttle ผ่าน `AITickManager` (ไม่ใช่ทุก frame) เพื่อ performance
- Navigation หยุดได้จาก 3 สาเหตุ: `_navigationPaused`, `_isMovementStopped`, `!NavigationAgent.HasValidFlow`
- เมื่อ damage ถูก register threat เพิ่ม ×3 ให้ target ที่โจมตี

---

## FlowField System

### ไฟล์หลัก
| ไฟล์ | หน้าที่ |
|------|---------|
| `3_Application/FlowField/FlowFieldManager.cs` | Singleton — build + cache fields |
| `3_Application/FlowField/FlowFieldBuilder.cs` | BFS จาก target cells เติม direction |
| `3_Application/FlowField/FlowFieldNavigationService.cs` | EnsureField ให้ enemy ใช้ |
| `3_Application/FlowField/FlowFieldOwner.cs` | Component บน enemy — ถือ footprint |
| `3_Application/FlowField/FlowFieldTarget.cs` | Component บน target — ถือ FlowKey |

### Flow
```
EnemyController.TickState
  → FlowFieldNavigationService.Instance.EnsureField(flowKey, footprint, targetPos)
      → FlowFieldManager.BuildField(channel, footprint, targets)
          → EnsureBounds() — auto-detect grid size จาก WorldTileManager
          → build cost field (Minkowski erosion ตาม footprint)
          → FlowFieldBuilder.BuildFromTargets → BFS เติม direction ทุก cell
          → cache ใน _fields[FlowFieldKey]

EnemyController.FixedUpdate
  → Steering.TickSteering(footprint) → อ่าน direction จาก FlowField
  → Locomotion.ApplySteering(result)
```

### ข้อควรระวัง
- Field ถูก cache ตาม `FlowFieldKey(channel, footprint)` — ถ้า target ย้าย ต้อง `RemoveField` ก่อน build ใหม่
- `IsCellPassableForFootprint` ใช้ `pivotOffset=(0,0)` ตอน build — ไม่ใช่ pivot จริงของ enemy (by design)

---

## NPC System

### ไฟล์หลัก
| ไฟล์ | หน้าที่ |
|------|---------|
| `3_Application/NPC/NpcController.cs` | Entry point — orchestrate FlowField navigation สำหรับ NPC |
| `3_Application/NPC/NpcSteering.cs` | อ่านทิศจาก FlowField (simple, ไม่มี separation) implements `IFlowKeyHolder` |
| `3_Application/NPC/NpcLocomotion.cs` | ขับ Rigidbody2D ด้วย speed/accel |
| `3_Application/NPC/State/NpcIdleState.cs` | รอ target |
| `3_Application/NPC/State/NpcFollowState.cs` | navigate ไปหา target ผ่าน FlowField |

### วิธีใช้บน Prefab
1. ใส่ `FlowFieldOwner` + `NpcSteering` + `NpcLocomotion` + `NpcController` บน GameObject
2. ใส่ `FlowFieldTarget` บน destination (ใช้ channel เดียวกับ enemy ได้ถ้า target เดียวกัน)
3. เรียก `NpcController.AssignTarget(transform)` เพื่อให้ NPC เริ่มเดิน

### Shared กับ Enemy
- `FlowFieldOwner` — component เดียวกัน
- `FlowFieldNavigationAgent` — generic agent จาก Phase 1
- `INavigationAgent` — interface เดียวกัน

---

## Wave System

### ไฟล์หลัก
| ไฟล์ | หน้าที่ |
|------|---------|
| `3_Application/Enemy/WaveRuntime.cs` | wrapper — delegate ไปยัง IWaveMode |
| `3_Application/Enemy/WaveModeFactory.cs` | สร้าง mode จาก WaveType |
| `3_Application/Enemy/SpawnScheduler.cs` | Tick timer → SpawnBatch |
| `3_Application/Enemy/NormalWaveMode.cs` | spawn ต่อเนื่อง |
| `3_Application/Enemy/BurstWaveMode.cs` | spawn เป็นกลุ่ม |
| `3_Application/Enemy/SingleWaveMode.cs` | spawn จนครบจำนวน → IsFinished |
| `2_Logic/Wave/WaveDefinition.cs` | SO data: WaveType + SpawnPatterns |
| `2_Logic/Wave/SpawnPattern.cs` | data: EnemyPool, Count, Interval, Radius |

### Flow
```
WaveRuntime.Tick(dt)
  → IWaveMode.Tick(dt)
      → SpawnScheduler.Tick(dt) (มีได้หลายตัวใน 1 wave)
          → timer หมด → SpawnBatch()
              → Random count ใน [Min, Max]
              → Random enemy จาก EnemyPool
              → Random position บน annulus (MinRadius–MaxRadius)
              → _spawner.Spawn(hash, pos)
              → enemy.AssignTarget(GlobalTargetProvider.Instance.Player)
```

### WaveType
| Type | พฤติกรรม |
|------|----------|
| Normal | spawn ซ้ำตลอด |
| Burst | spawn เป็นชุด แล้วหยุด |
| Single | spawn จนครบ KillCount → IsFinished=true |

---

## Skill System

### ไฟล์หลัก
| ไฟล์ | หน้าที่ |
|------|---------|
| `3_Application/Skill/SkillController.cs` | API: ActiveSkill, ActiveSelfSkill |
| `3_Application/Skill/SkillSpawnController.cs` | spawn skill object ใน world |
| `3_Application/Skill/SkillSelfController.cs` | apply skill บน owner โดยตรง (buff/heal) |
| `3_Application/Skill/ISkill.cs` | interface สำหรับ skill ทุกตัว |
| `2_Logic/Skill/SkillDefinition/` | data definition ของแต่ละ skill |
| `2_Logic/Skill/SkillPayload/` | payload ที่ส่งเข้า skill |

### Flow (spawn-based skill)
```
PlayerInteractor.TryExecute
  → SkillController.ActiveSkill(payload, owner, intent, skillDef, targetPos)
      → SkillSpawnController.ActiveSkill(...)
          → spawn skill GameObject ที่ targetPos
          → skill object execute logic ของตัวเอง
```

### Flow (self skill)
```
SkillController.ActiveSelfSkill(payload, intent)
  → SkillSelfController.Use(payload, intent)
      → apply effect บน energyable/owner โดยตรง
```

---

## Game Loop / State Machine

### ไฟล์หลัก
| ไฟล์ | หน้าที่ |
|------|---------|
| `3_Application/GameState/GameBootstrap.cs` | MonoBehaviour — สร้าง GameApplication + ส่ง Update |
| `3_Application/GameState/GameApplication.cs` | ถือ GameStateMachine + forward Update |
| `3_Application/GameState/GameLoop.cs` | รัน IGameSystem ทุกตัวที่ลงทะเบียน |
| `3_Application/GameState/GameStateMachine.cs` | จัดการ state transition + notify listeners |
| `3_Application/GameState/GameState.cs` | base class ของแต่ละ state |
| `3_Application/GameState/EGameState.cs` | enum: Upgrade, Gameplay, Inventory |

### States
| State | เมื่อไหร่ |
|-------|----------|
| GameplayState | ช่วงเล่นเกมปกติ |
| UpgradeState | เปิด altar / upgrade UI |
| InventoryState | เปิด inventory |

### Flow
```
GameBootstrap.Initialize(container)
  → BuildApplication: สร้าง states + GameStateMachine
  → container.Register ทุก state

GameBootstrap.StartGame
  → GameApplication.Start → ChangeState(Gameplay)

GameBootstrap.Update(dt)
  → GameApplication.Update → GameStateMachine.Update
      → currentState.Update → GameLoop.Update
          → IGameSystem.Update (ทุก system ที่ Add ไว้)
```

### ข้อควรระวัง
- `IGameStateListener` ต่างจาก `IGameSystem` — listener รับ event เปลี่ยน state, system รับ Update tick
- `GameLoop.AddSystem` ต้องเรียกตอน install (6_Installs) ก่อน StartGame

---

## คำแนะนำการ debug

1. **Bug เกี่ยวกับ interaction** → เริ่มที่ `ItemInteractionAction.cs` trace flow ลงมา
2. **Bug เกี่ยวกับ item ไม่เข้า inventory** → เช็ค `WorldInteractionExecutor.GiveRewards` + `PlayerInventory.AddItem`
3. **Bug เกี่ยวกับ cost/consume ผิด** → เช็ค `InteractionCostConfig` (SO) + `ApplyFeedback` ใน `ItemInteractionAction`
4. **Bug เกี่ยวกับ altar** → เช็ค `PlaceOfferingAction.CanProcess` + `RemoveOfferingAction.CanProcess` + `IsOccupied`
5. **Enemy ไม่ move** → เช็ค `FlowFieldNavigationService.EnsureField` + `ChaseState` + `_navigationPaused`
6. **Wave ไม่ spawn** → เช็ค `SpawnScheduler.Tick` + `WaveRuntime.IsFinished` + `GlobalTargetProvider.Instance`
7. **Skill ไม่ fire** → เช็ค `PlayerInteractor.TryExecute` → `SkillController.ActiveSkill`
8. **State ไม่เปลี่ยน** → เช็ค `GameStateMachine.ChangeState` + `IGameStateListener.OnGameStateChanged`
9. **ไม่รู้ว่าไฟล์ไหน** → Grep ชื่อ class ใน `Assets/Scripts/` ก่อนเสมอ
