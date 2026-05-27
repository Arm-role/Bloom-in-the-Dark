#nullable enable

using UnityEngine;

// SO เดี่ยวต่อ 1 hint — designer สร้างจาก CreateAssetMenu แล้วใส่ไว้ใน HintLibrary._entries
// Id เป็น unique string — convention: snake_case (e.g., "intro_pickup", "altar_offering")
[CreateAssetMenu(menuName = "Hint/Hint Entry")]
public sealed class HintEntry : ScriptableObject, IHintEntry
{
  [Header("Identity")]
  [SerializeField] private string _id = string.Empty;

  [Header("Content")]
  [SerializeField] private string _title = string.Empty;

  [TextArea(3, 10)]
  [SerializeField] private string _description = string.Empty;

  [SerializeField] private Sprite? _media;

  [Header("Filter / Unlock")]
  [SerializeField] private HintCategory _category = HintCategory.General;
  [SerializeField] private bool _unlockedByDefault;

  public string Id => _id;
  public string Title => _title;
  public string Description => _description;
  public Sprite? Media => _media;
  public HintCategory Category => _category;
  public bool UnlockedByDefault => _unlockedByDefault;
}
