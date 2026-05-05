public sealed class ConeConfig : ITargetingConfig
{
  public float XAngle { get; }
  public float Range { get; }
  public float AngleDeg { get; }

  public ConeConfig(float xAngle, float range, float angleDeg)
  {
    XAngle = xAngle;
    Range = range;
    AngleDeg = angleDeg;
  }
}