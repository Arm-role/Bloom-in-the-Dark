using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController :
  MonoBehaviour,
  IGameStateListener,
  IDamageable,
  IEnergyable,
  IDestructible,
  IPoolable<GameObject>
{

  [SerializeField] private Transform AnimationViewRoot;

  private Rigidbody2D _rb;

  private IPlayerInput _playerInput;
  private IStatDatabase _statDatabase;

  private IMovement _playerMovement;
  private PlayerState _playerState;
  private IStatService _statService;

  private PlayerHealth _playerHealth;

  private KnockbackSimulator _knockback;

  private BarPresenter<PlayerHealth> _healthPresenter;
  private BarPresenter<PlayerEnergy> _energyPresenter;

  private CharacterAnimationSystem _playerAnimationSystem;

  public PlayerEnergy PlayerEnergy { get; private set; }

  public PlayerInteractor Interactor { get; private set; }

  private IBarView BarView { get; set; }
  private IBarView EnergyBarView { get; set; }

  private IFlashHitView _flashHitView;

  public event Action<CharacterDamageResult> OnDamaged;
  public event Action<PlayerEnergyResult> OnEnergy;

  public event Action<GameObject> OnRequestDestruction;

  public bool IsAlive { get; set; } = true;
  public bool isGamePlayStat;

  private void Awake()
  {
    _flashHitView = GetComponent<IFlashHitView>();

    foreach (var bar in GetComponents<IBarView>())
    {
      if (bar.Name == "HP")
      {
        BarView = bar;
      }
      else if (bar.Name == "Energy")
      {
        EnergyBarView = bar;
      }
    }

    _rb = GetComponent<Rigidbody2D>();
  }

  public void Initialize(
    IPlayerInput playerInput,
    IStatDatabase statDatabase,
    PlayerState playerState,
    IStatService statService,
    PlayerHealth playerHealth,
    PlayerEnergy playerEnergy,
    PlayerInteractor interactor,
    CharacterAnimationSystem animationSystem)
  {
    _playerInput = playerInput;
    _statDatabase = statDatabase;
    _playerState = playerState;
    _statService = statService;

    _playerHealth = playerHealth;

    _knockback = GetComponent<KnockbackSimulator>();

    Interactor = interactor;

    _playerAnimationSystem = animationSystem;
    _playerAnimationSystem.Initializa(AnimationViewRoot.GetComponent<ICharacterAnimationView>());

    _healthPresenter = new BarPresenter<PlayerHealth>(playerHealth, BarView);
    _energyPresenter = new BarPresenter<PlayerEnergy>(playerEnergy, EnergyBarView);

    _playerMovement = new PlayerMovement(_statDatabase.MoveSpeed, _statService);

    _playerState.OnMoveDirection += _playerAnimationSystem.SetMoveDirection;
    _playerState.OnLookDirection += _playerAnimationSystem.SetLookDirection;

    OnDamaged += _playerAnimationSystem.HandleDamage;
  }

  private void OnDisable()
  {
    _playerState.OnMoveDirection -= _playerAnimationSystem.SetMoveDirection;
    _playerState.OnLookDirection -= _playerAnimationSystem.SetLookDirection;

    OnDamaged -= _playerAnimationSystem.HandleDamage;
  }

  public void OnGameStateChanged(EGameState state)
  {
    isGamePlayStat = state == EGameState.Gameplay || state == EGameState.Inventory;
  }

  public void Update()
  {
    if (!isGamePlayStat)
      return;

    if (Interactor != null && Interactor.IsBusy())
    {
      _playerState.UpdateMoveDirection(Vector2.zero);
      return;
    }

    _playerState.UpdateMoveDirection(_playerInput.MoveDirection);
  }

  public void FixedUpdate()
  {
    if (!isGamePlayStat) return;

    if (Interactor != null && Interactor.IsBusy())
    {
      _rb.velocity = Vector2.zero;
      return;
    }

    if (_knockback != null && _knockback.IsKnockbacking)
      return;

    Vector2 direction = _playerInput.MoveDirection;
    Vector2 velocity = _playerMovement.CalculateVelocity(direction);
    _rb.velocity = velocity;
  }

  public void OnSpawnFromPool(GameObject ob) { }
  public void OnReturnToPool(GameObject ob) { }


  public void RequestDestruction()
  {
    OnRequestDestruction?.Invoke(gameObject);
  }

  public void TakeDamage(DamageContext context)
  {
    _flashHitView?.FlashEffect();

    Interactor.TryExecute(
      new TakeDamageCommand(context.Damage)
    );

    if (context.KnockForce != 0 && context.KnockDration != 0)
      _knockback?.ApplyKnockback(context.HitDirection, context.KnockForce, context.KnockDration);

    bool isDead = !_playerHealth.IsAlive;

    var result = new CharacterDamageResult(
     context.Damage,
     transform.position,
     context.HitDirection,
     isDead);

    OnDamaged?.Invoke(result);

    if (isDead)
      OnDied();
  }

  public void AddEnergy(EnergyContext context)
  {
    Interactor.TryExecute(
      new IncreaseEnergyCommand(context.Energy));

    var result = new PlayerEnergyResult(
     context.Energy,
     transform.position);

    OnEnergy?.Invoke(result);
  }

  public void Heal(float ammount)
  {
    _playerHealth.Heal(ammount);
  }

  public void SetMaxHp(float ammount)
  {
    _playerHealth.SetMax(ammount);
  }

  public void HpFill()
  {
    _playerHealth.Fill();
  }

  private void OnDied()
  {
    RequestDestruction();
  }
}
