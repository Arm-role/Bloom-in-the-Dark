using System;
using System.Collections;
using UnityEngine;

public interface IStoryboardView
{
  event Action OnAdvanceRequested;
  bool IsTyping { get; }
  void ShowPanel(Sprite image, string text, float charsPerSecond);
  void CompleteText();
  IEnumerator FadeIn(float duration);
  IEnumerator FadeOut(float duration);
}
