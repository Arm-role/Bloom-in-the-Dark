#nullable enable

// Config สำหรับ inventory SFX — concrete impl คือ InventorySoundConfig (SO ใน 4_Infrastructure)
// แยก interface เข้า 2_Logic เพราะ 3_Application ไม่ ref 4_Infrastructure ได้
public interface IInventorySoundConfig
{
  SoundKey? OnPick { get; }
  SoundKey? OnPlace { get; }
  SoundKey? OnSwap { get; }
  SoundKey? OnQuickMove { get; }
  SoundKey? OnFail { get; }
  SoundKey? OnHotbarSelect { get; }
  SoundKey? OnOpen { get; }
  SoundKey? OnClose { get; }
}
