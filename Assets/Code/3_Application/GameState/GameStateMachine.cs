using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStateMachine
{
  private readonly Dictionary<EGameState, IGameState> _states;
  private IGameState _current;

  public EGameState CurrentState { get; private set;  }

  public GameStateMachine(IEnumerable<IGameState> states)
  {
    _states = states.ToDictionary(s => s.Type);
  }

  public void ChangeState(EGameState type)
  {
    if (_current?.Type == type) return;

    //Debug.Log(type.ToString());

    _current?.Exit();
    _current = _states[type];
    _current.Enter();

    CurrentState = type;
  }

  public void Tick()
  {
    _current?.Tick();
  }

  public void FixedTick()
  {
    _current?.FixedTick();
  }
}