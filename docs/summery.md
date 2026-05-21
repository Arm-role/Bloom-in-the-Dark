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

ทุกระบบหลักมีไฟล์ doc แยกในโฟลเดอร์ `docs/` นี้ — เปิดไฟล์ตามตาราง (ครบ flow + contract + gotcha)

| ระบบ | เอกสาร | Entry point |
|------|--------|-------------|
| Animation | `animation.md` | `CharacterAnimationSystem.HandleDamage` |
| Player + Respawn | `player.md` | `PlayerController.TakeDamage` → `OnDied` |
| Pooling + Spawner | `pooling.md` | `SpawnerHandle.SpawnAsync` / `Despawn` |
| Interaction | `interaction.md` | `ItemInteractionAction.ProcessInteractionContext` |
| Item Capability | `interaction.md` | `ItemInteractionCapability.TryGetInteractionRule` |
| Enemy AI | `enemy.md` | `EnemyController` + State machine |
| Game Loop / State | `game-state.md` | `GameStateMachine.ChangeState` |
| Inventory | `inventory.md` | `PlayerInventory.AddItem` |
| Offering Altar | `altar.md` | `OfferingAltarController.TryPlaceItem` |
| FlowField | `flow-field.md` | `FlowFieldNavigationService.EnsureField` |
| Cycle (Wave spawn) | `cycle.md` | `CycleController.StartCycle` |
| Skill | `skill.md` | `SkillController.ActiveSkill` |
| NPC | `npc.md` | `NpcController.AssignTarget` |

> เพิ่ม/ย้ายระบบ ใช้ `_TEMPLATE.md` เป็นแม่แบบ — anchor ด้วยชื่อ symbol ห้ามใส่ line number

---

## Interaction System

→ ย้ายไป **`interaction.md`** — flow, ICellAction, cost, contracts, gotchas ครบ (รวม Item Capability)

---

## Inventory System

→ ย้ายไป **`inventory.md`** — AddItem flow, contracts, gotchas ครบ

---

## Offering Altar

→ ย้ายไป **`altar.md`** — place/remove flow, AltarDomain mode, contracts, gotchas ครบ

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

ส่วน altar ย้ายไป `altar.md` แล้ว — ส่วนที่เหลือ (`PlayerProgression`, stat services, `GlobalUpgradeDomain`) ยังไม่ได้ทำ doc

---

## Enemy / AI

→ ย้ายไป **`enemy.md`** — lifecycle, state machine, combat, contracts, gotchas ครบ

---

## FlowField System

→ ย้ายไป **`flow-field.md`** — build/cache flow, contracts, gotchas ครบ

---

## NPC System

→ ย้ายไป **`npc.md`** — state machine, navigation, contracts, gotchas ครบ

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
