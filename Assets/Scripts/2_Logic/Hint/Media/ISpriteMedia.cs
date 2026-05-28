#nullable enable

using UnityEngine;

// Static image — designer drag Sprite asset ลง SpriteMediaData SO
public interface ISpriteMedia : ITutorialMedia
{
  Sprite Sprite { get; }
}
