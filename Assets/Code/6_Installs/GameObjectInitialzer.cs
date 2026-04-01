using UnityEngine;

public class GameObjectInitialzer
{
  private readonly TurnSystem _turnSystem;
  private readonly SpawnerHandle _spawnerHandle;
  private readonly WorldInteractionExecutor _executor;
  private readonly FloatingTextService _floatingTextService;

  public GameObjectInitialzer(
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

  private void Subscribe(GameObject obj)
  {
    if (obj.TryGetComponent<EnemyController>(out var c))
    {
      c.OnGetLootable += Execute;
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
  }

  private void UnSubscribe(GameObject obj)
  {
    if (obj.TryGetComponent<EnemyController>(out var c))
    {
      c.OnGetLootable -= Execute;
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
    {
      damageable.OnDamaged += SpawnDamageText;
    }

    if (obj.TryGetComponent<IEnergyable>(out var energyable))
    {
      energyable.OnEnergy += SpawnEnergyText;
    }

    if (obj.TryGetComponent<IHealthable>(out var healthable))
    {
      healthable.OnHeal += SpawnHealText;
    }
  }

  // =======================
  // Helper
  // =======================

  private async void Execute(WorldAction action)
  {
    await _executor.Execute(action);
  }

  private async void SpawnDamageText(CharacterDamageResult result)
  {
    await _floatingTextService.Spawn(
              FloatingTextType.Damage,
              result.Hitbox,
              result.Damage
          );
  }

  private async void SpawnHealText(PlayerHealthResult result)
  {
    await _floatingTextService.Spawn(
              FloatingTextType.Heal,
              result.Hitbox,
              result.Amount
          );
  }

  private async void SpawnEnergyText(PlayerEnergyResult result)
  {
    await _floatingTextService.Spawn(
              FloatingTextType.Energy,
              result.Hitbox,
              result.Energy
          );
  }

  private async void SpawnExpText(PlayerExpResult result)
  {
    await _floatingTextService.Spawn(
              FloatingTextType.Exp,
              result.Hitbox,
              result.Energy
          );
  }
}