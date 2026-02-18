public sealed class DirectInteractConfig : ITargetingConfig
{
  public float MaxDistance { get; }

  public DirectInteractConfig(float maxDistance)
  {
    MaxDistance = maxDistance;
  }
}