using UnityEngine;

public interface IFloatingTextStyleConfig
{
  FloatingTextType Tpye { get; }
  Color Color { get; }
  float Lifetime { get; }

  AnimationCurve OpacityCurve { get; }
  AnimationCurve ScaleCurve { get; }
  AnimationCurve HightCurve { get; }
}