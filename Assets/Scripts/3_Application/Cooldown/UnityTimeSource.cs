using UnityEngine;

public sealed class UnityTimeSource : ITimeSource
{
  public float Now => Time.time;
}