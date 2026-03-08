using UnityEngine;

public interface IEnemySpawner
{
    void Spawn(EnemyType type, Vector3 position);
}