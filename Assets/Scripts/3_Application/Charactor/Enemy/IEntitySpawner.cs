using System.Threading.Tasks;
using UnityEngine;

public interface IEntitySpawner
{
  Task<EntityController> Spawn(int id, Vector3 position);
  Task<NpcController> SpawnNpc(int id, Vector3 position);
}