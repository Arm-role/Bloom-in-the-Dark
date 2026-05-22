# Bloom-in-the-Dark — Script Navigation Guide

อ่านไฟล์นี้ก่อนเสมอก่อนเปิดไฟล์ใดๆ เพื่อลด token — เป็น **index** ชี้ไปไฟล์ doc ของแต่ละระบบ

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

ทุกระบบมีไฟล์ doc แยกในโฟลเดอร์ `docs/` นี้ — เปิดไฟล์ตามตาราง (ครบ flow + contract + gotcha)

| ระบบ | เอกสาร | Entry point |
|------|--------|-------------|
| Animation | `animation.md` | `CharacterAnimationSystem.HandleDamage` |
| Player + Respawn | `player.md` | `PlayerController.TakeDamage` → `OnDied` |
| Enemy AI | `enemy.md` | `EnemyController` + State machine |
| NPC | `npc.md` | `NpcController.AssignTarget` |
| Pooling + Spawner | `pooling.md` | `SpawnerHandle.SpawnAsync` / `Despawn` |
| Cycle (Wave spawn) | `cycle.md` | `CycleController.StartCycle` |
| FlowField | `flow-field.md` | `FlowFieldNavigationService.EnsureField` |
| Interaction | `interaction.md` | `ItemInteractionAction.ProcessInteractionContext` |
| Item Capability | `interaction.md` | `ItemInteractionCapability.TryGetInteractionRule` |
| Inventory | `inventory.md` | `PlayerInventory.AddItem` |
| Offering Altar | `altar.md` | `OfferingAltarController.TryPlaceItem` |
| Skill | `skill.md` | `SkillController.ActiveSkill` |
| Game Loop / State | `game-state.md` | `GameStateMachine.ChangeState` |
| Stats | `stats.md` | `IStatService.GetStat` |
| Progression / Upgrade | `progression.md` | `PlayerProgression.AddExp` |
| Defeat / End game | `defeat.md` | `BaseBuildingController.TakeDamage` |
| Camera | `camera.md` | `CameraController.SetState` |
| Character feedback | `character-feedback.md` | `KnockbackSimulator` / `BarPresenter` |

> เพิ่มระบบใหม่ ใช้ `_TEMPLATE.md` เป็นแม่แบบ — anchor ด้วยชื่อ symbol ห้ามใส่ line number

---

## ยังไม่มี doc

ระบบที่ยังไม่ได้ทำ doc — Grep ชื่อ class ใน `Assets/Scripts/` ก่อน (เจอแล้วทำ doc ใหม่จาก `_TEMPLATE.md`):

- World / Grid / Tile (`WorldTileManager`, `WorldCell`, `GridLogic`)
- Plant growth (`PlantGrowthController`, `PlantProgressionDomain`)
- Audio (`AudioBootstrap`, `AudioService`)
- UI views ส่วนใหญ่ใน `5_Views/`
- DI installers ใน `6_Installs/`
- Interaction cost ฝั่ง state: `InteractionRuntimeState` (cooldown dict)

---

## คำแนะนำการ debug

| อาการ | เริ่มดูที่ | doc |
|-------|-----------|-----|
| Interaction ไม่ทำงาน | `ItemInteractionAction.ProcessInteractionContext` trace ลงมา | `interaction.md` |
| Item ไม่เข้า inventory | `WorldInteractionExecutor` + `PlayerInventory.AddItem` | `inventory.md` |
| Cost/consume ผิด | `InteractionCostConfig` (SO) + `ApplyFeedback` | `interaction.md` |
| Altar bug | `PlaceOfferingAction.CanProcess` + `AltarDomain` mode | `altar.md` |
| Enemy ไม่ move | `FlowFieldNavigationService.EnsureField` + `ChaseState` | `enemy.md` / `flow-field.md` |
| Enemy ไม่ spawn | `CycleController.StartCycle` + `CycleRuntime.Tick` | `cycle.md` |
| Skill ไม่ fire | `PlayerInteractor.TryExecute` → `SkillController.ActiveSkill` | `skill.md` |
| State ไม่เปลี่ยน | `GameStateMachine.ChangeState` + `IGameStateListener` | `game-state.md` |
| Stat ไม่อัปเดต | `GlobalUpgradeDomain.AddUpgrade` → `IStatService` | `stats.md` / `progression.md` |
| ไม่รู้ว่าไฟล์ไหน | Grep ชื่อ class ใน `Assets/Scripts/` ก่อนเสมอ | — |
