using System.Collections.Generic;

public class GameLoop
{
  private readonly List<IGameSystem> _systems = new();

  public void AddSystem(IGameSystem system)
  {
    _systems.Add(system);
  }

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