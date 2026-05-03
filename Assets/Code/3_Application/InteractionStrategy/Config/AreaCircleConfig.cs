public sealed class AreaCircleConfig : ITargetingConfig
{
  public float Range { get; }
  public float Radius { get; }
  public float XAngle { get; }

  public AreaCircleConfig(float range, float radius, float xAngle)
  {
    Range = range;
    Radius = radius;
    XAngle = xAngle;
  }
}