using UnityEngine;

public class SpawnMock : MonoBehaviour
{
    private SpawnerHandle _spawnHandle;

    public void Initialze(SpawnerHandle spawner)
    {
        _spawnHandle = spawner;
    }

    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Vector2 pointer = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            await _spawnHandle.SpawnAsync("Enemy", pointer);
        }
    }
}