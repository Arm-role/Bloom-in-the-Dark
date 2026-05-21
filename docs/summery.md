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

## 🗺️ Index — เริ่มที่นี่

ระบบที่มีไฟล์ doc แยก → อ่านไฟล์นั้นแทน (อยู่ในโฟลเดอร์ `docs/` เดียวกันนี้ — ครบ flow + contract + gotcha)
ระบบที่ยังไม่ย้าย → อ่าน section ด้านล่างในไฟล์นี้

| ระบบ | เอกสาร | Entry point |
|------|--------|-------------|
| Animation | `animation.md` | `CharacterAnimationSystem.HandleDamage` |
| Player + Respawn | `player.md` | `PlayerController.TakeDamage` → `OnDied` |
| Pooling + Spawner | `pooling.md` | `SpawnerHandle.SpawnAsync` / `Despawn` |
| Interaction | `interaction.md` | `ItemInteractionAction.ProcessInteractionContext` |
| Item Capability | `interaction.md` | `ItemInteractionCapability.TryGetInteractionRule` |
| Enemy AI | `enemy.md` | `EnemyController` + State machine |
| Game Loop / State | `game-state.md` | `GameStateMachine.ChangeState` |
| Inventory | § ในไฟล์นี้ | `PlayerInventory.AddItem` |
| Offering Altar | § ในไฟล์นี้ | `OfferingAltarController.TryPlaceItem` |
| FlowField | `flow-field.md` | `FlowFieldNavigationService.EnsureField` |
| Cycle (Wave spawn) | `cycle.md` | `CycleController.StartCycle` |
| Skill | `skill.md` | `SkillController.ActiveSkill` |
| NPC | § ในไฟล์นี้ | `NpcController.AssignTarget` |

> เพิ่ม/ย้ายระบบ ใช้ `_TEMPLATE.md` เป็นแม่แบบ — anchor ด้วยชื่อ symbol ห้ามใส่ line number

---

## Interaction System

→ ย้ายไป **`interaction.md`** — flow, ICellAction, cost, contracts, gotchas ครบ (รวม Item Capability)

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

→ ย้ายไป **`interaction.md`** — รวมกับ Interaction (เป็น rule data ที่ป้อน Interaction)

---

## Player Systems

→ ย้ายไป **`player.md`** แล้ว — death/respawn flow, contracts, gotchas ครบ

ที่เหลือ (ฝั่ง interaction cost):
| ไฟล์ | หน้าที่ |
|------|---------|
| `3_Application/Interactable/InteractionRuntimeState.cs` | cooldown dictionary |
| `3_Application/Interactable/InteractionCostResolver.cs` | TryResolve + ApplyCost |

---

## Progression / Upgrade

โฟลเดอร์: `2_Logic/Progression/Upgrade/` และ `3_Application/Progression/Upgrade/`

ยังไม่ได้ explore ละเอียด — ถ้าต้องการ ดู `AltarController`, `AltarDomain` ก่อน

---

## Enemy / AI

→ ย้ายไป **`enemy.md`** — lifecycle, state machine, combat, contracts, gotchas ครบ

---

## FlowField System

→ ย้ายไป **`flow-field.md`** — build/cache flow, contracts, gotchas ครบ

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

## Wave / Cycle System

→ ย้ายไป **`cycle.md`** — Wave runtime ถูก refactor เป็น Cycle (day-based) ; `WaveRuntime`/`SpawnScheduler`/`WaveMode*` ถูกลบแล้ว

---

## Skill System

→ ย้ายไป **`skill.md`** — spawn-based + self skill flow, contracts, gotchas ครบ

---

## Game Loop / State Machine

→ ย้ายไป **`game-state.md`** — flow, contracts, gotchas ครบ

---

## คำแนะนำการ debug

1. **Bug เกี่ยวกับ interaction** → เริ่มที่ `ItemInteractionAction.cs` trace flow ลงมา
2. **Bug เกี่ยวกับ item ไม่เข้า inventory** → เช็ค `WorldInteractionExecutor.GiveRewards` + `PlayerInventory.AddItem`
3. **Bug เกี่ยวกับ cost/consume ผิด** → เช็ค `InteractionCostConfig` (SO) + `ApplyFeedback` ใน `ItemInteractionAction`
4. **Bug เกี่ยวกับ altar** → เช็ค `PlaceOfferingAction.CanProcess` + `RemoveOfferingAction.CanProcess` + `IsOccupied`
5. **Enemy ไม่ move** → เช็ค `FlowFieldNavigationService.EnsureField` + `ChaseState` + `_navigationPaused`
6. **Enemy ไม่ spawn** → เช็ค `CycleController.StartCycle` + `CycleRuntime.Tick` + `GlobalTargetProvider.Instance` (ดู `cycle.md`)
7. **Skill ไม่ fire** → เช็ค `PlayerInteractor.TryExecute` → `SkillController.ActiveSkill`
8. **State ไม่เปลี่ยน** → เช็ค `GameStateMachine.ChangeState` + `IGameStateListener.OnGameStateChanged`
9. **ไม่รู้ว่าไฟล์ไหน** → Grep ชื่อ class ใน `Assets/Scripts/` ก่อนเสมอ
