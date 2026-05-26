#nullable enable

using UnityEngine;

// Per-action SFX สำหรับ inventory UI — field nullable ทุกตัว
// designer ใส่เฉพาะ slot ที่ต้องการเสียง, field ที่เว้นไว้ silent
[CreateAssetMenu(menuName = "Audio/Inventory Sound Config")]
public sealed class InventorySoundConfig : ScriptableObject, IInventorySoundConfig
{
  [Header("Slot interactions")]
  [SerializeField] private SoundKey? _onPick;
  [SerializeField] private SoundKey? _onPlace;     // empty target OR merge same-Data
  [SerializeField] private SoundKey? _onSwap;      // diff-Data swap (จะ fallback ไป OnPlace ถ้า null)
  [SerializeField] private SoundKey? _onQuickMove;
  [SerializeField] private SoundKey? _onFail;      // QuickMove returns false (เต็ม / ไม่มีช่อง)

  [Header("Hotbar")]
  [SerializeField] private SoundKey? _onHotbarSelect;

  [Header("Inventory screen")]
  [SerializeField] private SoundKey? _onOpen;
  [SerializeField] private SoundKey? _onClose;

  public SoundKey? OnPick => _onPick;
  public SoundKey? OnPlace => _onPlace;
  public SoundKey? OnSwap => _onSwap ?? _onPlace;  // fallback
  public SoundKey? OnQuickMove => _onQuickMove;
  public SoundKey? OnFail => _onFail;
  public SoundKey? OnHotbarSelect => _onHotbarSelect;
  public SoundKey? OnOpen => _onOpen;
  public SoundKey? OnClose => _onClose;
}
