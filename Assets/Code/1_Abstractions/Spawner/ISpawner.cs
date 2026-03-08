using System.Threading.Tasks;
using UnityEngine;

public interface ISpawner
{
  public Task<GameObject> SpawnAsync(string itemName, Vector3 position);

  public Task<GameObject> SpawnAsync(string itemName, Vector3 position, Vector3 direction);

  public Task<GameObject> SpawnAsync(int id, Vector3 position);

  public Task<GameObject> SpawnAsync(int id, Vector3 position, Vector3 direction);

  public void Despawn(GameObject Ob);
}