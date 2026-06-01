#nullable enable

using UnityEngine;

// SO เดี่ยวต่อ 1 hint — designer สร้างจาก CreateAssetMenu แล้วลากใส่ slot ใน HintLibrary
//
// Id = ชื่อ asset (filename) — ไม่ต้องตั้งเอง, rename = Id เปลี่ยน (save key เปลี่ยนตาม)
//   ห้ามใส่ '|' ในชื่อ asset (delimiter ของ PlayerPrefs ใน HintState)
//
// Media field:
//   _mediaData (TutorialMediaData SO) → polymorphic Sprite/etc
//   _media (legacy Sprite) → ทำงานต่อได้ via LegacySpriteAdapter (backward compat)
//   ถ้าตั้งทั้งสอง → _mediaData ชนะ
[CreateAssetMenu(menuName = "Hint/Hint Entry")]
public sealed class HintEntry : ScriptableObject, IHintEntry
{
  [Header("Content")]
  [SerializeField] private string _title = string.Empty;

  [Header("Media (เลือก 1 — Data field ชนะถ้าตั้งทั้งสอง)")]
  [Tooltip("Polymorphic — ลาก SpriteMediaData asset ลงที่นี่")]
  [SerializeField] private TutorialMediaData? _mediaData;

  [Tooltip("Legacy Sprite — ใช้ตอน entry เก่ายังไม่ได้ migrate เป็น SpriteMediaData")]
  [SerializeField] private Sprite? _media;

  [Header("Filter / Unlock")]
  [SerializeField] private HintCategory _category = HintCategory.General;
  [SerializeField] private bool _unlockedByDefault;

  [Tooltip("ถ้า true → HintUnlockBinder จะยิง popup auto ตอน event ครั้งแรก " +
           "(default false = silent unlock, player เปิด menu ดูเอง)")]
  [SerializeField] private bool _autoShowOnUnlock;

  // cache adapter ของ legacy sprite — กัน allocate ทุก Media access
  private LegacySpriteAdapter? _legacyCache;

  public string Id => name;
  public string Title => _title;
  public HintCategory Category => _category;
  public bool UnlockedByDefault => _unlockedByDefault;
  public bool AutoShowOnUnlock => _autoShowOnUnlock;

  public ITutorialMedia? Media
  {
    get
    {
      if (_mediaData != null) return _mediaData;
      if (_media == null) return null;
      _legacyCache ??= new LegacySpriteAdapter(_media);
      return _legacyCache;
    }
  }

}
