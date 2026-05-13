using UnityEngine;

public class UnityLootRandom : ILootRandom
{
  public int Range(int min, int max)
  {
    return Random.Range(min, max);
  }

  public float Value()
  {
    return Random.value;
  }
}