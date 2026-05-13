using UnityEngine;

public interface IPrefabResolver
{
  GameObject Resolve(string prefabId);
}