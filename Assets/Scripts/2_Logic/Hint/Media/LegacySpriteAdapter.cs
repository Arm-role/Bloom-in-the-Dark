#nullable enable

using UnityEngine;

// Adapter สำหรับ backward compat — wrap Sprite (HintEntry._media field เดิม) เป็น ISpriteMedia
// ใช้ภายใน HintEntry getter ตอน _mediaData ยังไม่มีแต่ _media (Sprite) มี
// pure C# value type — ไม่มี Unity state, ปลอดภัย allocate ตอน access
public sealed class LegacySpriteAdapter : ISpriteMedia
{
  public Sprite Sprite { get; }

  public LegacySpriteAdapter(Sprite sprite)
  {
    Sprite = sprite;
  }
}
