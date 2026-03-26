using UnityEngine;

public interface IEnemySpawner
{
    void Spawn(int id, Vector3 position, float moveSpeed = 3f, int hp = 10);
}