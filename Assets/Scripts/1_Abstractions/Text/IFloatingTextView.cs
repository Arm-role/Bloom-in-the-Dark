using System;
using UnityEngine;

public interface IFloatingTextView
{
  void Setup(
     string message,
     Color color,
     float lifetime,
     AnimationCurve opacity,
     AnimationCurve scale,
     AnimationCurve hight,
     Action<GameObject> onFinished);
}
