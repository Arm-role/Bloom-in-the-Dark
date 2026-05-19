using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Storyboard")]
public class StoryboardSO : ScriptableObject
{
  public StoryboardPanel[] panels;
}

[Serializable]
public struct StoryboardPanel
{
  public Sprite image;
  [TextArea(3, 8)] public string text;
  [Min(0.1f)] public float charsPerSecond;
}
