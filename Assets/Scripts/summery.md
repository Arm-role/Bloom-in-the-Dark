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

โฟลเดอร์: `3_Application/Charactor/Enemy/`

ระบบ state machine: IdleState → ChaseState → AttackState
Combat: EnemyCombat, EnemyPatternBrain, AOESlamPattern, DashAttackPattern

---

## คำแนะนำการ debug

1. **Bug เกี่ยวกับ interaction** → เริ่มที่ `ItemInteractionAction.cs` trace flow ลงมา
2. **Bug เกี่ยวกับ item ไม่เข้า inventory** → เช็ค `WorldInteractionExecutor.GiveRewards` + `PlayerInventory.AddItem`
3. **Bug เกี่ยวกับ cost/consume ผิด** → เช็ค `InteractionCostConfig` (SO) + `ApplyFeedback` ใน `ItemInteractionAction`
4. **Bug เกี่ยวกับ altar** → เช็ค `PlaceOfferingAction.CanProcess` + `RemoveOfferingAction.CanProcess` + `IsOccupied`
5. **ไม่รู้ว่าไฟล์ไหน** → Glob `**/*<ClassName>*` ก่อนเสมอ
