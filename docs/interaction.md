# Interaction System

ระบบ click/interact กับ world — รวม Item Capability (rule data) + ICellAction

## Entry point

`ItemInteractionAction.cs` → `ProcessInteractionContext()` — trace ทุก interaction เริ่มที่นี่

> `ItemInteractionAction` เป็น **orchestrator บางๆ** — งานจริงกระจายไป 3 collaborator (preview / resolver / action runner)

## Files

| symbol / class | path | หน้าที่ |
|----------------|------|---------|
| `ItemInteractionAction` | `3_Application/InteractionStrategy/ItemInteractionAction.cs` | Orchestrator — route input phase + track item ที่เลือก |
| `InteractionPreviewController` | `3_Application/InteractionStrategy/InteractionPreviewController.cs` | preview indicator lifecycle (resolve/show/update/hide) |
| `InteractionResolver` | `3_Application/InteractionStrategy/InteractionResolver.cs` | resolve strategy→bundle→target→validate + global fallback |
| `InteractionActionRunner` | `3_Application/InteractionStrategy/InteractionActionRunner.cs` | run action: prepare→cost→animation→commit→feedback (async) |
| `CellInteractionPipeline` | `3_Application/Factory/CellInteractionPipeline.cs` | Resolve + Process `ICellAction` ของ cell |
| `WorldInteractionExecutor` | `3_Application/Interactable/Executor/WorldInteractionExecutor.cs` | Apply rewards / damage / tiles |
| `InteractionCostResolver` | `3_Application/Interactable/InteractionCostResolver.cs` | `TryResolve` cost → `InteractionFeedback` |
| `InteractionCostConfig` | `4_Infrastructure/Interacable/InteractionCostConfig.cs` | SO: cost entries (intent + tags + target) |
| `GameActionFactory` | `3_Application/Factory/GameActionFactory.cs` | Register `ICellAction` ให้แต่ละ object |
| `ItemInteractionCapability` | `4_Infrastructure/Item/Modules/ItemInteractionCapability.cs` | SO: `InteractionRules` + `PreviewRules` |
| `InteractionRule` | `2_Logic/Interacable/InteractionRule/InteractionRule.cs` | Input, PhaseMask, Condition, Fallback, IntentType, Strategy |

## Flow

```
DragDropController.OnInteraction → ItemInteractionAction.ProcessInteractionContext(ctx)
  → SyncState — item เปลี่ยน? → OnItemChanged → push capability ให้ _preview + _resolver
  → ไม่มี item / capability==null → ProcessPhases → InteractionResolver.HandleGlobalInteraction
  → มี item → ProcessPhases(InteractionPreviewController.Handle) → _preview.Tick
              → ProcessPhases(InteractionResolver.HandleInteraction)

InteractionResolver.HandleInteraction(input, phase)   [วน 3 phase: Pressed / Held / Released]
  → hover UI → ข้าม
  → capability.TryGetInteractionRule(input, phase, ctx, out rule)
       ไม่เจอ → HandleGlobalInteraction (fallback)
       เจอ    → ExecuteTargeted(rule, input)

InteractionResolver.ExecuteTargeted(rule, input)
  → rule.Strategy==None → ถ้า rule.Fallback==Global → ExecuteTargetedGlobal
  → bundle = InteractionHandleService.Resolve(rule.Strategy)
  → target = bundle.Targeting.Strategy.Resolve(ctx, config)
  → valid = target.IsValid && Validator?.Validate && bundle.Action.CanExecute(intent, target)
  → ไม่ valid + Fallback==Global → ExecuteTargetedGlobal
  → InteractionActionRunner.Execute(intent, bundle, target)

InteractionActionRunner.Execute → ExecuteAsync (async Task, fire-and-forget)
  → guard: interactor.IsBusy / cooldown / มี _pendingPlan ค้าง → return
  → plan = await bundle.Action.Prepare(owner, intent, target)
  → costResolver.TryResolve(...) → feedback  ;  CanAfford? (energy + item)
  → _pendingPlan = plan
  → มี animation tag → animationSystem.Handle(command) → รอ event
       ไม่มี → CommitPendingAsync ทันที

InteractionActionRunner.CommitPendingAsync   ← ผูกกับ RaiseImpact + RaiseFinished (OnAnimationCommit)
  → result = await plan.Commit()
  → result.Outcome==Consumed →
       result.Cell เป็น WorldCell → Execute(result.Action, cell)
       result.Cell == null (placement/skill) → Execute(result.Action)   [no-cell overload]
       ApplyFeedback — consume energy/item, apply cooldown
```

### CellInteractionPipeline (เรียกโดย plan ตอน resolve ICellAction)

```
Execute(intent, cell)
  → Resolve: วน InteractionStageOrder.All → cell.ActionRegistry.GetByStage(stage)
       → action ตัวแรกที่ ICellAction.CanProcess(intent, cell)==true
  → ICellAction.Process(intent, cell) → InteractionResult
```

## ICellAction — สร้างโดย GameActionFactory

`GameActionFactory.CreateActions` ลงทะเบียน action ตาม component / tile type (verified):

| Trigger | Actions ที่สร้าง |
|---------|------------------|
| `SoilTileData` (tile) | `RemoveSoilAction`, `PlantSeedAction` |
| `GrassTileData` (tile) | `TillGrassToSoilAction` |
| `PlantGrowthController` (object) | `PlantHarvestAction`, `RemoveSeedAction` |
| `ClearableState` (object) | `ClearableAction` |
| `OfferingAltarController` (object) | `PlaceOfferingAction`, `RemoveOfferingAction` |
| `IBuildingController` (object) | `DemolishBuildingAction` |

แต่ละ action มี property `Stage` (`InteractionStage`) — `CellInteractionPipeline.Resolve` วน `InteractionStageOrder.All` เลือก action แรกที่ `CanProcess` ผ่าน
เงื่อนไขเต็มอยู่ใน `CanProcess` ของแต่ละ action — ตัวอย่าง `PlaceOfferingAction`: input `Secondary` + `HasItem` + altar ยังไม่ occupied, `Stage = Pre`

## Contracts (public API — เปลี่ยนช้า เชื่อถือได้)

- `ItemInteractionAction`: `Dispose()` — implement `IDispose` (ctor ผูก `DragDropController.OnInteraction`; `Dispose` chain ไป `InteractionActionRunner.Dispose`)
- `InteractionActionRunner`: `Execute(intent, bundle, target)` (fire-and-forget), `Dispose()` — ผูก/ปลด `RaiseImpact` + `RaiseFinished`
- `InteractionResolver`: `HandleInteraction` / `HandleGlobalInteraction` (เป็น phase handler), `SetItem(item, capability)`
- `InteractionPreviewController`: `Handle` / `Tick` / `Disable` / `SetProvider` / `IsActive`
- `CellInteractionPipeline`: `Execute(intent, cell) : Task<InteractionResult>`, `GetTargetMask(...)`, `Resolve(...) : Task<IGameAction>`
- `IItemInteractionCapability`: `TryGetInteractionRule(input, phase, ctx, out rule) : bool`, `GetPreviewRules(...)`

## Gotchas

- **action ถูก commit ผ่าน animation event** — `_pendingPlan` (อยู่ใน `InteractionActionRunner`) ค้างจนกว่า `RaiseImpact`/`RaiseFinished` จะยิง `CommitPendingAsync`; ถ้าไม่มี animation tag จะ commit ทันที
- มี `_pendingPlan` ค้าง 1 อันเท่านั้น — interaction ใหม่ถูก block จนกว่าอันเดิม commit
- `ExecuteAsync` / `CommitPendingAsync` เป็น `async Task` หุ้มด้วย adapter `void` (`Execute` / `OnAnimationCommit`) — **ไม่ใช่ `async void`** exception ถูก try/catch ครบทุก path
- `_preview` กับ `_resolver` รับ "item ที่เลือก" ผ่าน `OnItemChanged` → `SetProvider` / `SetItem` — orchestrator เป็นจุด sync จุดเดียว
- ไม่เจอ rule → fallback `HandleGlobalInteraction` (intent มาจาก `_globalConfig.Resolve`)
- cost ผูกกับ `intent.Type` ผ่าน `InteractionCostConfig` — action ที่มี cost ของตัวเอง (`InteractionResult.Cost`) จะ override config
- `TryGetInteractionRule` คืน rule แรกที่ match — ลำดับใน `InteractionRules` มีผล
- **cell-less Consumed result** — `PlacementActionPerformer`/`Skill`/`SelfUse` คืน `Consumed` โดย `Cell == null` (action ทำงานเองใน `Commit` แล้ว). `CommitPendingAsync` ต้อง route ด้วย `result.Cell is WorldCell` → ถ้าไม่มี cell ใช้ overload `Execute(action)` ห้าม cast `(WorldCell)null` ส่งเข้า `Execute(action, cell)` (NRE ที่ `WorldCenter` บรรทัดแรก + ข้าม `ApplyFeedback` → **ของที่วางไม่ถูกหักจาก inventory**). `Execute(action, cell)` มี guard `worldCell == null` กันซ้ำอีกชั้น

## Hit interrupt — Player โดนตีระหว่าง action

อาการที่เคย bite: player กดทำ action → hit animation แทรกก่อน RaiseImpact ของ action ยิง → action clip ถูก override → event ของ action ไม่ยิงเลย → `_pendingPlan` ค้าง → interaction ถัดไปทั้งหมดถูก block (`hasPendingPlan = true`)

**Fix:**
- `ItemInteractionAction` subscribe `PlayerController.OnDamaged` → เรียก `_actionRunner.CancelPending()` → reset `_pendingPlan = null`
- Unsubscribe ใน `Dispose` ตรงคู่กับ subscribe (กัน event leak)

**Cancel semantics:**
- Energy / item **ไม่ consume** ที่จุดนี้ (consume ใน `ApplyFeedback` หลัง Commit) → ไม่ต้อง refund
- Cooldown lock (`_interactor.TryStartAction(name, duration)`) ที่ตั้งไว้ก่อน animation ตัก timer ลงตามธรรมชาติ → ปล่อยไว้ ไม่ release
- ถ้า `_pendingPlan == null` อยู่แล้ว (commit ไปแล้ว / ไม่มี action) → `CancelPending()` เป็น no-op ปลอดภัยเรียกซ้ำ
- Hit damage **ไม่ refund cooldown** — โดน hit ตอน commit = เสีย action นั้น + รอ cooldown ต่อ (penalty คล้าย Dark Souls / Monster Hunter)

**ถ้า RaiseImpact ยิงไปแล้วก่อนโดน hit:** action commit ไปแล้ว (`_pendingPlan = null` ตั้งแต่ใน `CommitPendingAsync`) → `CancelPending()` no-op → behavior ถูกต้อง

## SFX (Phase 3)

`WorldInteractionExecutor` รับ `IAudioService?` + `IInteractionSoundConfig?` (optional) → เล่น 3D SFX ที่ `worldCell.WorldCenter` ตอน apply action

| Trigger | Config field | ที่เกิด |
|---------|--------------|---------|
| Tile place (till soil, plant seed → AddTile) | `OnTilePlace` | `Execute` ตอน `TryAddTile` สำเร็จ |
| Tile remove | `OnTileRemove` | `Execute` ตอน `TryRemoveTile` สำเร็จ |
| Destructible break (plant harvest, tree chop) | `OnDestructibleBreak` | `Execute` หลัง `destructible.ApplyDamage` คืน true |
| Reward grant (item เข้า inventory) | `OnPickup` | `Execute` หลัง `GiveRewards` (ครั้งเดียวต่อ action ไม่ใช่ต่อ item) |

### Position 3D
- `Execute(WorldAction, WorldCell)` ใช้ `worldCell.WorldCenter`
- `Execute(WorldAction)` overload (no cell) ใช้ `action.SourcePosition`
- ทั้งคู่เป็น Vector3 → SFX spatial ถ้า `SoundData.Is3D = true`

### Design
- **Global config** — เสียงเดียวสำหรับทุก action ของ type เดียวกัน (harvest plant = harvest tree, ที่ break เสียงเดียว) จุดเริ่มต้นง่าย ขยาย per-action ตอนจำเป็น
- **เสียง pickup ครั้งเดียวต่อ action** — ไม่ stack ตามจำนวน item (ถ้า reward 5 ชิ้น ดังเสียงเดียว)
- **OnDestructibleBreak ดังหลัง object เสียจริง** (`objectDestroyed = true`) — ไม่ใช่ทุกครั้งที่ตี (ตี 3 ทีถึงพัง ดังครั้งเดียวตอนพัง)

### Phase ถัดไป (ถ้าต้องการ per-action sound)
- เพิ่ม `SoundKey? ExecuteSfx` ใน `WorldAction` struct → ทุก action SO ตั้งเสียงตัวเอง override global
- หรือ per-`ICellAction` SO field — เช่น `PlantHarvestAction.HarvestSfx`, `ChopTreeAction.ChopSfx`

## Related

- `docs/altar.md` — `PlaceOfferingAction` / `RemoveOfferingAction`
- `docs/inventory.md` — `ApplyFeedback` consume item ผ่าน `PlayerInteractor`
- `docs/audio.md` — `IAudioService` + `IInteractionSoundConfig`
