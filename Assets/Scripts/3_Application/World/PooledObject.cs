using UnityEngine;

public class PooledObject : MonoBehaviour, IPooObject
{
  public int KeyId { get; private set; }

  public void Initialize(int ketId)
  {
    KeyId = ketId;
  }
}