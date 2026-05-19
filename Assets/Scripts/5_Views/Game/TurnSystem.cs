using System;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
  [SerializeField] private ETurnState defaultTurnState;

  public event Action<ETurnState> OnNextTurn;

  private ETurnState _turnState;
  private int _day = 1;

  private IStatService _statService;

  private StatKey _maxHpKey;
  private StatKey _hpRefill;

  private StatKey _maxEnegyKey;
  private StatKey _energyRefill;

  private GameTag _ownerTag;


  private IEnergyable _playerEnergy;
  private IHealthable _playerHealth;
  private IPlayerInteractor _interactor;
  private ICycleController _cycleController;
  private ITurnView _turnView;
  private bool _isTransitioning;

  public void Initialize(
    IStatDatabase statDatabase,
    IStatService phaseStatService,
    GameTag ownerTag,
    IEnergyable playerEnergy,
    IHealthable playerHealth,
    IPlayerInteractor interactor,
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

    // endless mode → เริ่มที่ day ที่กำหนด (default 51) แทนที่จะเป็น 1
    if (GameSession.IsEndlessMode)
      _day = GameSession.EndlessStartDay;

    SetTurn(defaultTurnState, true);
  }

  private void OnDisable()
  {
    _interactor.OnEnergyChanged -= OnCurrentEnergyChanged;
    _cycleController.OnCycleCompleted -= BattleCycleCompleted;
    _turnView.OnSkipTurn -= NextTurn;
    _statService.onUpgrade -= OnStatChanged;
  }

  private void OnStatChanged(GameTag tag, StatKey key)
  {
    if (tag.Hash != _ownerTag.Hash)
      return;

    if (key == _maxEnegyKey)
    {
      float newMax = _statService.GetStat(_maxEnegyKey);
      _interactor.SetMaxEnergy(newMax);
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
    else
      _turnView.HideSkipButton();
  }

  private void BattleCycleCompleted()
  {
    NextTurn();
  }

  private void NextTurn()
  {
    if (_isTransitioning) return;

    var nextState = _turnState switch
    {
      ETurnState.Farm         => ETurnState.Preparation,
      ETurnState.Preparation  => ETurnState.Battle,
      ETurnState.Battle       => ETurnState.Farm,
      _                       => ETurnState.Farm,
    };

    int nextDay = nextState == ETurnState.Farm ? _day + 1 : _day;
    string label = $"Day {nextDay}\n{nextState}";

    _isTransitioning = true;
    _turnView.PlayTurnTransition(
      label,
      onMidpoint: () => SetTurn(nextState),
      onComplete: () => _isTransitioning = false);
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
        _cycleController.StartCycle(_day);
        break;

      case ETurnState.Farm:
        _turnView.HideSkipButton();
        _cycleController.StopCycle();
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
    }
  }
}
