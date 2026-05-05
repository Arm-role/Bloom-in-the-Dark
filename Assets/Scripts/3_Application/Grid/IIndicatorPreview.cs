using UnityEngine;

public interface IIndicatorPreview
{
  void Initialize();
  void Enable();
  void Disable();
}

public interface IAreaCircleIndicatorPreview : IIndicatorPreview
{
  void UpdateView(Vector2 rangePos, Vector2 healPos,
                  Vector3 rangeScale, Vector3 healScale);
}

public interface IConeIndicatorPreview : IIndicatorPreview
{
  void UpdateView(
    Vector2 origin,
    Vector2 direction,
    Vector3 rangeScale,
    Vector3 coneScale,
    float angle);
}