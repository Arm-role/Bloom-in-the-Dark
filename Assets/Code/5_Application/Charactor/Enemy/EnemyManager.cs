using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    private List<EnemyController> _enemies = new List<EnemyController>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterEnemy(EnemyController enemy)
    {
        if (!_enemies.Contains(enemy)) _enemies.Add(enemy);
    }

    public void UnregisterEnemy(EnemyController enemy)
    {
        if (_enemies.Contains(enemy)) _enemies.Remove(enemy);
    }

    private void Update()
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            var e = _enemies[i];
            if (e != null) e.ManualUpdate();
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            var e = _enemies[i];
            if (e != null) e.ManualFixedUpdate();
        }
    }
}