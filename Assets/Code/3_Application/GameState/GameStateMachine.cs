using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStateMachine
{
  private readonly Dictionary<EGameState, GameState> _states;
  private readonly List<IGameStateListener> _listeners = new();
  private GameState _current;

  public EGameState CurrentState { get; private set;  }


  public GameStateMachine(IEnumerable<GameState> states)
  {
    _states = states.ToDictionary(s => s.State);
  }
  public void AddStateListener(IGameStateListener system)
  {
    _listeners.Add(system);
  }

  public void ChangeState(EGameState type)
  {
    if (_current?.State == type) return;

    _current?.Exit();
    _current = _states[type];
    _current.Enter();

    CurrentState = type;
    Debug.LogWarning("current State " + _current.ToString());

    foreach (var listener in _listeners)
      listener.OnGameStateChanged(CurrentState);
  }

  public void Update(float dt)
  {
    _current?.Update(dt);
  }

  public void FixedUpdate(float dt)
  {
    _current?.FixedUpdate(dt);
  }
}