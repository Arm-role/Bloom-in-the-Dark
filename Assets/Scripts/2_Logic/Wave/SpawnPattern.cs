using System;

[Serializable]
public class SpawnPattern
{
  public float MinRadius;
  public float MaxRadius;
  public float MinInterval;
  public float MaxInterval;
  public int MinCount;
  public int MaxCount;
  public EnemyEntry[] EnemyPool;
}
