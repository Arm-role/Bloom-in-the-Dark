using System.Collections.Generic;

public class GameLoop
{
  private readonly List<IGameSystem> _systems = new();

  public void AddSystem(IGameSystem system)
  {
    _systems.Add(system);
  }

  // ล้าง system ทั้งหมด — เรียกตอนเข้า scene ใหม่ ก่อน installer AddSystem รอบใหม่
  // กัน system สะสมทับ + กันเรียก Update บน MonoBehaviour ของ scene เก่าที่ถูกทำลายแล้ว
  public void Clear() => _systems.Clear();

  public void Enter()
  {
    foreach (var system in _systems)
      system.Enter();
  }

  public void Exit()
  {
    foreach (var system in _systems)
      system.Exit();
  }

  public void Update(float dt)
  {
    foreach (var system in _systems)
      system.Update(dt);
  }

  public void FixedUpdate(float dt)
  {
    foreach (var system in _systems)
      system.FixedUpdate(dt);
  }
}