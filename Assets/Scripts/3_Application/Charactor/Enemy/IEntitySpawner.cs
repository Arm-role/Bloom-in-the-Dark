using System.Threading.Tasks;
using UnityEngine;

public interface IEntitySpawner
{
  Task<EntityController> Spawn(int id, Vector3 position);
}