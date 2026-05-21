# Animation System

ระบบ animation ของตัวละคร (player + enemy) — facade เชื่อม controller ↔ Unity Animator

## Entry point

`CharacterAnimationSystem.cs` → `Handle()` / `HandleDamage()` — controller สั่ง animation ผ่านที่นี่

## Files

| symbol / class | path | หน้าที่ |
|----------------|------|---------|
| `CharacterAnimationSystem` | `3_Application/Animation/CharacterAnimationSystem.cs` | Facade — controller ถือตัวนี้ ไม่ยุ่งกับ view ตรง ๆ |
| `PlayerAnimation` | `5_Views/Charactor/PlayerAnimation.cs` | View ของ player (`ICharacterAnimationView`) |
| `EnemyAnimation` | `5_Views/Charactor/EnemyAnimation.cs` | View ของ enemy — มี `_hpBar` toggle เพิ่ม |
| `ICharacterAnimationView` | `1_Abstractions/Animation/` | Contract ของ view |
| `CharacterAnimationCommand` | `1_Abstractions/Animation/` | `.Tag` (`AnimationTag`) + `.TransitionDuration` |
| `ICharacterAnimationLibrary` | `1_Abstractions/Animation/` | ถือ `DeathTag`, `HitTag` |

## Flow

```
Controller → CharacterAnimationSystem.HandleDamage(result)
  → tag = result.IsDead ? library.DeathTag : library.HitTag
  → view.Play(command)         ← CrossFade เข้า clip
                                  *ไม่ auto-lock — controller ต้องสั่งเอง*

[clip มี Animation Event บน frame]
  → view.Animation_Finished()  → RaiseFinished
  → view.Animation_Impact()    → RaiseImpact
```

## Contracts (public API — เปลี่ยนช้า เชื่อถือได้)

`CharacterAnimationSystem`:
- events: `RaiseImpact`, `RaiseFinished` (forward จาก view)
- `Initializa(ICharacterAnimationView)` — ผูก view (สังเกตชื่อสะกดแบบนี้)
- `Handle(in CharacterAnimationCommand) : bool`
- `SetMoveDirection(Vector2)`, `SetLookDirection(Vector2)`
- `HandleDamage(CharacterDamageResult)` — เล่น Death หรือ Hit clip ตาม `result.IsDead`
- `LockAnimation()` / `UnlockAnimation()`
- `ShowVisual()` / `HideVisual()` / `Reset()`

`ICharacterAnimationView` (impl: `PlayerAnimation`, `EnemyAnimation`):
- `Play(command) : bool` — return `false` ถ้า `_locked` หรือ Animator ไม่มี state นั้น
- `Animation_Finished()` / `Animation_Impact()` — เรียกโดย Animation Event บน clip
- `HideVisual()` ปิด `SpriteRenderer.enabled` ทุกตัว (Enemy ปิด `_hpBar` ด้วย)
- `ResetAnimation()` — ปลด lock + เล่น `"Helmet_Idle"`

## Gotchas

- **`HandleDamage` ไม่ auto-lock** — controller ต้องเรียก `LockAnimation()` เอง (ดู `Enemy.DeadState.Enter`, `PlayerController.OnDied`) ถ้า lock auto จะค้างเป็น "fake death" lock
- **`RaiseFinished` เป็น event รวม** — clip ไหนก็ตามที่มี Animation Event `Animation_Finished` จะ raise → ถ้าไม่ `LockAnimation()` clip อื่น (walk/idle) จะยิง event ผิดจังหวะ
- `Play` ใช้ `CrossFade` — Player fixed `0.15s`, Enemy ใช้ `command.TransitionDuration`
- `HideVisual()` ปิดแค่ renderer **ไม่ปิด GameObject** — ตอน reuse/respawn ต้อง `ShowVisual()` กลับ ไม่งั้นตัวล่องหน
- `ResetAnimation()` hard-code clip `"Helmet_Idle"`

## Related

- `player.md` — Player ใช้ระบบนี้ตอน death/respawn
- `summery.md` § Enemy/AI — `DeadState` ใช้ `LockAnimation` + `RaiseFinished`
