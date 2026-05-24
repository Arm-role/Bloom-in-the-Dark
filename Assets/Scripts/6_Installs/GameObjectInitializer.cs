using UnityEngine;

public class GameObjectInitializer
{
  private readonly TurnSystem _turnSystem;
  private readonly SpawnerHandle _spawnerHandle;
  private readonly WorldInteractionExecutor _executor;
  private readonly FloatingTextService _floatingTextService;

  // late-bind — TradeController ถูกสร้างใน SceneBindingInstaller (หลัง GameObjectInitializer)
  private IPlayerInput _playerInput;
  private TradeController _tradeController;

  public GameObjectInitializer(
    TurnSystem turnSystem,
    SpawnerHandle spawnerHandle,
    WorldInteractionExecutor executor,
    FloatingTextService floatingTextService)
  {
    _turnSystem = turnSystem;
    _spawnerHandle = spawnerHandle;

    _executor = executor;
    _floatingTextService = floatingTextService;

    _spawnerHandle.OnSpawnCompleted += Subscribe;
    _spawnerHandle.OnDespawnCompleted += UnSubscribe;

    _executor.OnExpResult += SpawnExpText;
  }

  public void SetTradeContext(IPlayerInput playerInput, TradeController tradeController)
  {
    _playerInput = playerInput;
    _tradeController = tradeController;
  }

  private void Subscribe(GameObject obj)
  {
    if (obj.TryGetComponent<EnemyController>(out var c))
    {
      c.OnGetLootable += Execute;
    }

    if (obj.TryGetComponent<MerchantNpc>(out var merchant))
    {
      merchant.Initialize(_playerInput, _tradeController);
    }
    if (obj.TryGetComponent<IGrowthEntity>(out var growth))
    {
      _turnSystem.OnNextTurn += growth.OnTurnPassed;
    }

    if (obj.TryGetComponent<IDamageable>(out var damageable))
    {
      damageable.OnDamaged += SpawnDamageText;
    }

    if (obj.TryGetComponent<IDestructible>(out var des))
    {
      des.OnRequestDestruction += _spawnerHandle.Despawn;
    }

    if (obj.TryGetComponent<IBuildingController>(out var baseBuilding))
    {
      baseBuilding.Initialize(obj =>_executor.RemoveObject(obj));
    }
  }

  private void UnSubscribe(GameObject obj)
  {
    if (obj.TryGetComponent<EnemyController>(out var c))
    {
      c.OnGetLootable -= Execute;
    }

    if (obj.TryGetComponent<MerchantNpc>(out var merchant))
    {
      merchant.Teardown();
    }

    if (obj.TryGetComponent<IGrowthEntity>(out var growth))
    {
      _turnSystem.OnNextTurn -= growth.OnTurnPassed;
    }

    if (obj.TryGetComponent<IDamageable>(out var damageable))
    {
      damageable.OnDamaged -= SpawnDamageText;
    }

    if (obj.TryGetComponent<IDestructible>(out var des))
    {
      des.OnRequestDestruction -= _spawnerHandle.Despawn;
    }
  }

  public void ManualSubscribe(GameObject obj)
  {
    if (obj.TryGetComponent<IDamageable>(out var damageable))
      damageable.OnDamaged += SpawnDamageText;

    if (obj.TryGetComponent<IEnergyable>(out var energyable))
      energyable.OnEnergy += SpawnEnergyText;

    if (obj.TryGetComponent<IHealthable>(out var healthable))
      healthable.OnHeal += SpawnHealText;
  }

  public void ManualUnsubscribe(GameObject obj)
  {
    if (obj.TryGetComponent<IDamageable>(out var damageable))
      damageable.OnDamaged -= SpawnDamageText;

    if (obj.TryGetComponent<IEnergyable>(out var energyable))
      energyable.OnEnergy -= SpawnEnergyText;

    if (obj.TryGetComponent<IHealthable>(out var healthable))
      healthable.OnHeal -= SpawnHealText;
  }

  // =======================
  // Helper
  // =======================

  private async void Execute(WorldAction action)
  {
    try
    {
      await _executor.Execute(action);
    }
    catch (System.Exception ex)
    {
      Debug.LogError($"[GameObjectInitializer] Execute failed: {ex.Message}");
    }
  }

  private async void SpawnDamageText(CharacterDamageResult result)
  {
    try
    {
      await _floatingTextService.Spawn(
                FloatingTextType.Damage,
                result.Hitbox,
                result.Damage
            );
    }
    catch (System.Exception ex)
    {
      Debug.LogError($"[GameObjectInitializer] SpawnDamageText failed: {ex.Message}");
    }
  }

  private async void SpawnHealText(PlayerHealthResult result)
  {
    try
    {
      await _floatingTextService.Spawn(
                FloatingTextType.Heal,
                result.Hitbox,
                result.Amount
            );
    }
    catch (System.Exception ex)
    {
      Debug.LogError($"[GameObjectInitializer] SpawnHealText failed: {ex.Message}");
    }
  }

  private async void SpawnEnergyText(PlayerEnergyResult result)
  {
    try
    {
      await _floatingTextService.Spawn(
                FloatingTextType.Energy,
                result.Hitbox,
                result.Energy
            );
    }
    catch (System.Exception ex)
    {
      Debug.LogError($"[GameObjectInitializer] SpawnEnergyText failed: {ex.Message}");
    }
  }

  private async void SpawnExpText(PlayerExpResult result)
  {
    try
    {
      await _floatingTextService.Spawn(
                FloatingTextType.Exp,
                result.Hitbox,
                result.Exp
            );
    }
    catch (System.Exception ex)
    {
      Debug.LogError($"[GameObjectInitializer] SpawnExpText failed: {ex.Message}");
    }
  }
}