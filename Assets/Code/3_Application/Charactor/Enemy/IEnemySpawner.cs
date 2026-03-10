using UnityEngine;

public interface IEnemySpawner
{
    void Spawn(EnemyType type, Vector3 position, float moveSpeed = 3f, int hp = 10);
}