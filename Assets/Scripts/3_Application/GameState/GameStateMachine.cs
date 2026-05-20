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

  // เรียกตอนเข้า scene GamePlay ใหม่ ก่อน installer populate รอบใหม่
  // state machine + states เป็น DDOL อยู่ถาวร แต่ system/listener เป็น scene object
  // ที่ตายไปพร้อม scene เก่า — ไม่ล้างจะสะสมทับ + เรียก callback บน object ที่ถูกทำลาย
  public void ResetForNewScene()
  {
    foreach (var state in _states.Values)
      state.ClearSystems();

    _listeners.Clear();

    // null ทิ้งโดยไม่เรียก Exit() — system เก่าเป็น destroyed MonoBehaviour แล้ว
    // ให้ ChangeState(Gameplay) รอบใหม่จาก StartGame รัน Enter ได้จริง (ไม่ถูก early-return)
    _current = null;
  }

  public void ChangeState(EGameState type)
  {
    if (_current?.State == type) return;

    _current?.Exit();
    _current = _states[type];
    _current.Enter();

    CurrentState = type;

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