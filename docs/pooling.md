# Pooling + Spawner System

Object pool แบบ async (Addressables) + ชั้น spawner ที่ห่อ pool ไว้

## Entry point

`SpawnerHandle.cs` → `SpawnAsync()` / `Despawn()` — gameplay code เรียกผ่านที่นี่

## Files

| symbol / class | path | หน้าที่ |
|----------------|------|---------|
| `SpawnerHandle` | `3_Application/Spawner/SpawnerHandle.cs` | ชั้นบนสุด — เรียก `IPoolable` hooks + ยิง event |
| `GameObjectSpawner` | `4_Infrastructure/Spawner/GameObjectSpawner.cs` | `ISpawner` — แปลง id ↔ prefab, set active/position |
| `AdressablePoolingService` | `4_Infrastructure/Pooling/AdressablePoolingService.cs` | `IAdressablePoolService` — pool ต่อ prefab |
| `AsyncObjectPool<T>` | `4_Infrastructure/Pooling/AsyncObjectPool.cs` | Queue pool generic |
| `AdressbleGameObjectFactory` | `4_Infrastructure/Pooling/AdressbleGameObjectFactory.cs` | สร้าง instance จาก Addressables |
| `IPoolable<T>` | `1_Abstractions/Pooling/IPoolable.cs` | `IsAlive`, `OnSpawnFromPool`, `OnReturnToPool` |
| `ParticalService` | `4_Infrastructure/Spawner/ParticalService.cs` | spawn VFX ผ่าน pool |

## Flow — Spawn

```
SpawnerHandle.SpawnAsync(id, pos)
  → GameObjectSpawner.SpawnAsync
      → SpawnLocal: GameObjectLibrary.Find(id) → poolService.AsyncGet(prefab)
          → AdressablePoolingService: pool ต่อ prefab → AsyncObjectPool.GetAsync
              • pool ว่าง → factory.CreateAsync (Addressables)
              • ไม่ว่าง → Dequeue
          → _activeInstances[instance] = prefab
      → IPooObject.Initialize(id) ; instance.SetActive(true)
  → SpawnerHandle.RegisterPool: IPoolable.OnSpawnFromPool() ; IsAlive=true
```

## Flow — Despawn

```
IDestructible.RequestDestruction() → OnRequestDestruction event
  → (wired ใน 6_Installs/GameObjectInitializer) SpawnerHandle.Despawn(ob)
      → IPoolable.OnReturnToPool() ; IsAlive=false
      → GameObjectSpawner.Despawn
          → IPooObject.KeyId → poolService.Return(prefab, ob)
              → AdressablePoolingService.Return: instance.SetActive(false) → pool.Return (Enqueue)
```

## Contracts (public API — เปลี่ยนช้า เชื่อถือได้)

`SpawnerHandle`:
- events: `OnSpawnCompleted(GameObject)`, `OnDespawnCompleted(GameObject)`
- `SpawnAsync(int id, Vector3 pos [, Vector2 dir]) : Task<GameObject>`
- `Despawn(GameObject)`, `Register(GameObject)`

`IAdressablePoolService<T>`:
- `AsyncGet(prefab) : Task<GameObject>`, `Return(prefab, instance)`, `ReturnAll()`

`IPoolable<T>`: `IsAlive`, `OnSpawnFromPool(T)`, `OnReturnToPool(T)`

## Gotchas

- pool root (`[AdressablePoolingService_Root]`) เป็น **DontDestroyOnLoad** — instance ค้างข้าม scene ได้
- `ReturnAll()` — เรียกตอนเปลี่ยน scene คืน instance ที่ค้าง (projectile/VFX/skill กลางอากาศที่ไม่ถูก despawn ปกติ)
- object ที่ไม่มี `IPooObject` หรือ `KeyId == 0` → `Object.Destroy` ตรง ๆ ไม่เข้า pool (เช่น base building ที่วางใน scene)
- `OnSpawnFromPool`/`OnReturnToPool` ใช้ reset state ตอน reuse — เช่น `EnemyCombat` ล้าง skill list ไม่ให้สะสมจาก "ชาติก่อน"
- **Player ไม่เข้า despawn pool** — ใช้ respawn แทน (ดู `player.md`)
- iterate enemy ตอน clear ต้องทำบน copy — `RequestDestruction → Despawn → OnReturnToPool → UnregisterEnemy` แก้ collection ระหว่าง loop (ดู `EnemyManager`)

## Consumers

Enemy (`EnemyController`), Skill executors (`Cone/Line/Area/Poison...Executor`), `FloatingTextService`, `ParticalService`/VFX

## Related

- `summery.md` § Wave System — `SpawnScheduler` เรียก spawner
- `summery.md` § Enemy/AI — `EnemyController.OnSpawnFromPool` lifecycle
