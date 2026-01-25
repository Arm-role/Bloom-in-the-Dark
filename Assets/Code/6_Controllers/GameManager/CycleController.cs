using System;
using UnityEngine;

public class CycleController : MonoBehaviour
{
    [SerializeField] private CycleData cycleData;
    [SerializeField] private EnemySpawner spawner;

    public event Action OnCycleCompleted;

    private CycleRuntime _runtime;
    private bool _isRunning;

    public void StartCycle()
    {
        Debug.Log("Starting Cycle");
        _runtime = new CycleRuntime(
            cycleData,
            spawner,
            spawner.EnemyCounter   // สำคัญ
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