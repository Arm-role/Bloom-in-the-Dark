using UnityEngine;

public readonly struct AreaCirclePreviewData
{
  public readonly Vector2 Origin;
  public readonly Vector2 Center;
  public readonly Vector3 RangeScale;
  public readonly Vector3 AreaScale;

  public AreaCirclePreviewData(
      Vector2 origin,
      Vector2 center,
      Vector3 rangeScale,
      Vector3 areaScale)
  {
    Origin = origin;
    Center = center;
    RangeScale = rangeScale;
    AreaScale = areaScale;
  }
}

public readonly struct AreaLinePreviewData
{
  public readonly Vector2 Origin;
  public readonly Vector2 End;
  public readonly Vector3 Scale;
  public readonly float Angle;

  public AreaLinePreviewData(Vector2 origin, Vector2 end, Vector3 scale, float angle)
  {
    Origin = origin;
    End = end;
    Scale = scale;
    Angle = angle;
  }
}

public readonly struct ConePreviewData
{
  public readonly Vector2 Origin;
  public readonly Vector2 Direction;
  public readonly Vector3 RangeScale;
  public readonly Vector3 ConeScale;
  public readonly float Angle;

  public ConePreviewData(
      Vector2 origin,
      Vector2 direction,
      Vector3 rangeScale,
      Vector3 coneScale,
      float angle)
  {
    Origin = origin;
    Direction = direction;
    RangeScale = rangeScale;
    ConeScale = coneScale;
    Angle = angle;
  }
}