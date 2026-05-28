#nullable enable

using UnityEngine;

// Static sprite media — designer drag Sprite asset ลง field _sprite
[CreateAssetMenu(menuName = "Hint/Media/Sprite")]
public sealed class SpriteMediaData : TutorialMediaData, ISpriteMedia
{
  [SerializeField] private Sprite _sprite = null!;

  public Sprite Sprite => _sprite;
}
