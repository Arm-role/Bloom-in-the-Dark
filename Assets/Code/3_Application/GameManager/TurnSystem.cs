using System;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
  [SerializeField] private ETurnState defaultTurnState;

  public event Action<ETurnState> OnNextTurn;

  private ETurnState _turnState;
  private int _day = 1;

  private PlayerEnergy _playerEnergy;
  private CycleController _cycleController;
  private ITurnView _turnView;

  public void Initialize(
      PlayerEnergy playerEnergy,
      CycleController cycleController,
      ITurnView turnView)
  {
    _playerEnergy = playerEnergy;
    _cycleController = cycleController;
    _turnView = turnView;

    _turnView.OnSkipTurn += NextTurn;
    _turnView.HideSkipButton();

    _playerEnergy.OnChanged += OnCurrentEnergyChanged;
    _cycleController.OnCycleCompleted += BattleCycleCompleted;

    SetTurn(defaultTurnState, true);
  }

  private void OnDisable()
  {
    _playerEnergy.OnChanged -= OnCurrentEnergyChanged;
    _cycleController.OnCycleCompleted -= BattleCycleCompleted;
  }

  private void OnCurrentEnergyChanged(ResourceChangedEvent e)
  {
    if (e.Current <= 5)
      _turnView.ShowSkipButton();
  }

  private void BattleCycleCompleted()
  {
    NextTurn();
  }

  private void NextTurn()
  {
    switch (_turnState)
    {
      case ETurnState.Farm:
        SetTurn(ETurnState.Preparation);
        break;

      case ETurnState.Preparation:
        SetTurn(ETurnState.Battle);
        break;

      case ETurnState.Battle:
        SetTurn(ETurnState.Farm);
        break;
    }
  }

  private void SetTurn(ETurnState newState, bool isInit = false)
  {
    _turnState = newState;

    if (!isInit && newState == ETurnState.Farm)
    {
      _day++;
    }

    _turnView.SetTurnView(_day, _turnState.ToString());
    OnNextTurn?.Invoke(_turnState);

    HandleTurnEnter(newState);
  }

  private void HandleTurnEnter(ETurnState state)
  {
    switch (state)
    {
      case ETurnState.Battle:
        _turnView.HideSkipButton();
        _cycleController.StartCycle();
        break;

      case ETurnState.Farm:
        _turnView.HideSkipButton();
        _playerEnergy.ReFill();
        break;

      case ETurnState.Preparation:
        _turnView.HideSkipButton();
        _playerEnergy.ReFillAdd();
        break;

      default:
        _cycleController.StopCycle();
        break;
    }
  }
}
