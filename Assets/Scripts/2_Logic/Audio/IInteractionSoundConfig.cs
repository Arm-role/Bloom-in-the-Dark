#nullable enable

// Config สำหรับ world interaction SFX — pickup, destructible break, tile change
// concrete impl คือ InteractionSoundConfig (SO ใน 4_Infrastructure)
public interface IInteractionSoundConfig
{
  SoundKey? OnPickup { get; }              // ของเข้า inventory (reward grant)
  SoundKey? OnDestructibleBreak { get; }   // tree/plant ถูกทำลาย
  SoundKey? OnTilePlace { get; }           // tile ถูกเพิ่ม (till soil ฯลฯ)
  SoundKey? OnTileRemove { get; }          // tile ถูกลบ
}
