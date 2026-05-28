#nullable enable

using UnityEngine;

// SO เดี่ยวต่อ 1 hint — designer สร้างจาก CreateAssetMenu แล้วลากใส่ slot ใน HintLibrary
//
// Id ถูก auto-generate (GUID) ตอนสร้าง asset — designer ไม่ต้องสนใจ
// ถ้าอยาก override id (เช่น "welcome" สำหรับ readable debug) ยังแก้ได้ใน inspector
//
// Media field:
//   _mediaData (TutorialMediaData SO) → polymorphic Sprite/etc
//   _media (legacy Sprite) → ทำงานต่อได้ via LegacySpriteAdapter (backward compat)
//   ถ้าตั้งทั้งสอง → _mediaData ชนะ
[CreateAssetMenu(menuName = "Hint/Hint Entry")]
public sealed class HintEntry : ScriptableObject, IHintEntry
{
  [Tooltip("Auto-generated (GUID) ตอนสร้าง asset — override เฉพาะถ้าต้องการ id ที่อ่านได้ (เช่น 'welcome')")]
  [SerializeField] private string _id = string.Empty;

  [Header("Content")]
  [SerializeField] private string _title = string.Empty;

  [TextArea(3, 10)]
  [SerializeField] private string _description = string.Empty;

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

  public string Id => _id;
  public string Title => _title;
  public string Description => _description;
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

  // Auto-fill id ตอนสร้าง asset ครั้งแรก — designer ไม่ต้องตั้งเอง
  private void Reset()
  {
    _id = System.Guid.NewGuid().ToString();
  }

#if UNITY_EDITOR
  // Safety net — ถ้า designer เผลอ clear _id หรือ asset เก่าที่ยังไม่มี id → fill ใหม่
  private void OnValidate()
  {
    if (string.IsNullOrEmpty(_id))
      _id = System.Guid.NewGuid().ToString();
  }
#endif
}
