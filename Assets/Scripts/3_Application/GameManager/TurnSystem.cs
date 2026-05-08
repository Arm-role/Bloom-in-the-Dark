using System;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
  [SerializeField] private ETurnState defaultTurnState;

  public event Action<ETurnState> OnNextTurn;

  private ETurnState _turnState;
  private int _day = 1;

  private PhaseStatService _statService;

  private StatKey _maxHpKey;
  private StatKey _hpRefill;

  private StatKey _maxEnegyKey;
  private StatKey _energyRefill;

  private GameTag _ownerTag;


  private IEnergyable _playerEnergy;
  private IHealthable _playerHealth;
  private PlayerInteractor _interactor;
  private ICycleController _cycleController;
  private ITurnView _turnView;

  public void Initialize(
    IStatDatabase statDatabase,
    PhaseStatService phaseStatService,
    GameTag ownerTag,
    IEnergyable playerEnergy,
    IHealthable playerHealth,
    PlayerInteractor interactor,
    ICycleController cycleController,
    ITurnView turnView)
  {
    _statService = phaseStatService;
    _maxHpKey = statDatabase.MaxHp;
    _hpRefill = statDatabase.HpRefill;
    _maxEnegyKey = statDatabase.MaxEnergy;
    _energyRefill = statDatabase.EnergyRefill;
    _ownerTag = ownerTag;

    _playerEnergy = playerEnergy;
    _playerHealth = playerHealth;
    _interactor = interactor;
    _cycleController = cycleController;
    _turnView = turnView;

    _turnView.OnSkipTurn += NextTurn;
    _turnView.HideSkipButton();

    _interactor.OnEnergyChanged += OnCurrentEnergyChanged;
    _cycleController.OnCycleCompleted += BattleCycleCompleted;
    _statService.onUpgrade += OnStatChanged;

    SetTurn(defaultTurnState, true);
  }

  private void OnDisable()
  {
    _interactor.OnEnergyChanged -= OnCurrentEnergyChanged;
    _cycleController.OnCycleCompleted -= BattleCycleCompleted;
  }

  private void OnStatChanged(GameTag tag, StatKey key)
  {
    if (tag.Hash != _ownerTag.Hash)
      return;

    if (key == _maxEnegyKey)
    {
      float newMax = _statService.GetStat(_maxEnegyKey);
      _interactor.SetMaxEnegy(newMax);
    }
    else if (key == _maxHpKey)
    {
      float newMax = _statService.GetStat(_maxHpKey);
      _interactor.SetMaxHealth(newMax);
    }
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
        _playerEnergy.EnergyFill();

        var hpCalStat = _statService.GetStat(_hpRefill);
        var finalHpRefill = Mathf.RoundToInt(hpCalStat);

        _playerHealth.Heal(new HealthContext(finalHpRefill));
        break;

      case ETurnState.Preparation:
        _turnView.HideSkipButton();

        var energyCalStat = _statService.GetStat(_energyRefill);
        var finalEnergyRefill = Mathf.RoundToInt(energyCalStat);

        _playerEnergy.AddEnergy(new EnergyContext(finalEnergyRefill));
        break;

      default:
        _cycleController.StopCycle();
        break;
    }
  }
}
