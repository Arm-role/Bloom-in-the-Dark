using System;
using UnityEngine;

public class CycleController : MonoBehaviour, ICycleController
{
  [SerializeField] private WaveDefinition wave;
  [SerializeField] private EntitySpawner spawner;

  public event Action OnCycleCompleted;
  public event Action OnBossKilled;

  private CycleRuntime _runtime;
  private bool _isRunning;

  public void StartCycle(int day)
  {
    _runtime = new CycleRuntime(
        wave,
        spawner,
        spawner.EnemyCounter,
        day);

    _runtime.OnBossKilled += HandleBossKilled;

    _isRunning = true;
  }

  public void StopCycle()
  {
    if (_runtime != null)
      _runtime.OnBossKilled -= HandleBossKilled;

    _isRunning = false;
    _runtime = null;
  }

  private void HandleBossKilled() => OnBossKilled?.Invoke();

  private void Update()
  {
    if (!_isRunning || _runtime == null)
      return;

    _runtime.Tick(Time.deltaTime);

    if (_runtime.CanEndCycle)
    {
      _isRunning = false;
      OnCycleCompleted?.Invoke();
    }
  }
}
