using System;
using UnityEngine;

public class CycleController : MonoBehaviour, ICycleController
{
    [SerializeField] private WaveDefinition[] waves;
    [SerializeField] private EntitySpawner spawner;

    public event Action OnCycleCompleted;

    private CycleRuntime _runtime;
    private bool _isRunning;

    public void StartCycle()
    {
        _runtime = new CycleRuntime(
            waves,
            spawner,
            spawner.EnemyCounter
        );

        _isRunning = true;
    }

    public void StopCycle()
    {
        _isRunning = false;
        _runtime = null;
    }

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
