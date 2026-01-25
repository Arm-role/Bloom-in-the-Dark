using System;

[Serializable]
public class SpawnPattern
{
    public float MinInterval;
    public float MaxInterval;
    public int MinCount;
    public int MaxCount;
    public EnemyType[] EnemyPool;
}