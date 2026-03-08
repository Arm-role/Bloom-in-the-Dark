public sealed class AreaLineConfig : ITargetingConfig
{
  public float XAngle { get; }
  public float Length { get; }
  public float Width { get; }

  public AreaLineConfig(float xAngle, float length, float width)
  {
    XAngle = xAngle;
    Length = length;
    Width = width;
  }
}