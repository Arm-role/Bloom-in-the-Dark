using System;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystem : MonoBehaviour
{
  [SerializeField] private ETurnState defaultTurnState;

  [Header("UI")]
  [SerializeField] private Button nextTurnButton;

  public event Action<ETurnState> OnNextTurn;
  private ETurnState _turnState;

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

    nextTurnButton.onClick.AddListener(NextTurn);
    nextTurnButton.gameObject.SetActive(false);

    _playerEnergy.OnChanged += OnCurrentEnergyChanged;
    _cycleController.OnCycleCompleted += BattleCycleCompleted;

    SetTurn(defaultTurnState);
  }

  private void OnDisable()
  {
    _playerEnergy.OnChanged -= OnCurrentEnergyChanged;
    _cycleController.OnCycleCompleted -= BattleCycleCompleted;
  }

  private void OnCurrentEnergyChanged(ResourceChangedEvent e)
  {
    if (e.Current <= 5)
      nextTurnButton.gameObject.SetActive(true);
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

  private void SetTurn(ETurnState newState)
  {
    _turnState = newState;
    _turnView.SetTurnView(_turnState.ToString());
    OnNextTurn?.Invoke(_turnState);

    HandleTurnEnter(newState);
  }

  private void HandleTurnEnter(ETurnState state)
  {
    switch (state)
    {
      case ETurnState.Battle:
        nextTurnButton.gameObject.SetActive(false);
        _cycleController.StartCycle();
        break;

      case ETurnState.Farm:
        nextTurnButton.gameObject.SetActive(false);
        _playerEnergy.ReFill();
        break;

      default:
        _cycleController.StopCycle();
        break;
    }
  }
}
