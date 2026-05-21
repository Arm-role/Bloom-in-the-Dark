# Interaction System

ระบบ click/interact กับ world — รวม Item Capability (rule data) + ICellAction

## Entry point

`ItemInteractionAction.cs` → `ProcessInteractionContext()` — trace ทุก interaction เริ่มที่นี่

## Files

| symbol / class | path | หน้าที่ |
|----------------|------|---------|
| `ItemInteractionAction` | `3_Application/InteractionStrategy/ItemInteractionAction.cs` | Orchestrator ทั้งหมด |
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
  → SyncState — item เปลี่ยน? → OnItemChanged (โหลด capability + preview)
  → ไม่มี item / capability==null → ProcessPhases → HandleGlobalInteraction
  → มี item → ProcessPhases(HandlePreview) → TickPreview → ProcessPhases(HandleInteraction)

HandleInteraction(input, phase)              [วน 3 phase: Pressed / Held / Released]
  → hover UI → ข้าม
  → capability.TryGetInteractionRule(input, phase, ctx, out rule)
       ไม่เจอ → HandleGlobalInteraction (fallback)
       เจอ    → ExecuteTargeted(rule, input)

ExecuteTargeted(rule, input)
  → rule.Strategy==None → ถ้า rule.Fallback==Global → ExecuteTargetedGlobal
  → bundle = InteractionHandleService.Resolve(rule.Strategy)
  → target = bundle.Targeting.Strategy.Resolve(ctx, config)
  → valid = target.IsValid && Validator?.Validate && bundle.Action.CanExecute(intent, target)
  → ไม่ valid + Fallback==Global → ExecuteTargetedGlobal
  → ExecuteAction(intent, bundle, target)

ExecuteAction (async)
  → guard: interactor.IsBusy / cooldown / มี _pendingPlan ค้าง → return
  → plan = await bundle.Action.Prepare(owner, intent, target)
  → costResolver.TryResolve(...) → feedback   ;  CanAfford? (energy + item)
  → _pendingPlan = plan
  → มี animation tag → playerAnimationSystem.Handle(command) → รอ event
       ไม่มี → CommitPendingAction() ทันที

CommitPendingAction()        ← ผูกกับ RaiseImpact + RaiseFinished ของ animation
  → result = await plan.Commit()
  → result.Outcome==Consumed →
       WorldInteractionExecutor.Execute(result.Action, cell)
       ApplyFeedback — consume energy/item, apply cooldown
```

### CellInteractionPipeline (เรียกโดย plan ตอน resolve ICellAction)

```
Execute(intent, cell)
  → Resolve: วน InteractionStageOrder.All → cell.ActionRegistry.GetByStage(stage)
       → action ตัวแรกที่ ICellAction.CanProcess(intent, cell)==true
  → ICellAction.Process(intent, cell) → InteractionResult
```

## ICellAction (stage = Pre ทั้งหมด)

| Action | เงื่อนไข |
|--------|----------|
| `PlaceOfferingAction` | HasItem + UsePlace tag + altar ว่าง |
| `RemoveOfferingAction` | altar occupied |
| `PlantHarvestAction` | plant growth controller, harvestable |
| `RemoveSeedAction` (object/tile) | plant not harvestable / SoilTile มี seed |
| `ClearableAction` | ClearableState |
| `PlantSeedAction` (tile) | SoilTile + HasItem + seed tag |
| `TillGrassToSoilAction` (tile) | GrassTile + tool tag |
| `RemoveSoilAction` (tile) | SoilTile + ไม่มี seed |

## Contracts (public API — เปลี่ยนช้า เชื่อถือได้)

`ItemInteractionAction`: `Dispose()` — implement `IDispose` (constructor ผูก `DragDropController.OnInteraction` + `RaiseImpact/RaiseFinished`)
`CellInteractionPipeline`: `Execute(intent, cell) : Task<InteractionResult>`, `GetTargetMask(...)`, `Resolve(...) : Task<IGameAction>`
`IItemInteractionCapability`: `TryGetInteractionRule(input, phase, ctx, out rule) : bool`, `GetPreviewRules(...)`

## Gotchas

- **action ถูก commit ผ่าน animation event** — `_pendingPlan` ค้างจนกว่า `RaiseImpact`/`RaiseFinished` จะยิง `CommitPendingAction`; ถ้าไม่มี animation tag จะ commit ทันที
- มี `_pendingPlan` ค้าง 1 อันเท่านั้น — interaction ใหม่ถูก block จนกว่าอันเดิม commit
- ไม่เจอ rule → fallback `HandleGlobalInteraction` (intent มาจาก `_globalConfig.Resolve`)
- cost ผูกกับ `intent.Type` ผ่าน `InteractionCostConfig` — action ที่มี cost ของตัวเอง (`InteractionResult.Cost`) จะ override config
- `TryGetInteractionRule` คืน rule แรกที่ match — ลำดับใน `InteractionRules` มีผล

## Related

- `docs/altar.md` — `PlaceOfferingAction` / `RemoveOfferingAction`
- `summery.md` § Inventory — `ApplyFeedback` consume item ผ่าน `PlayerInteractor`
