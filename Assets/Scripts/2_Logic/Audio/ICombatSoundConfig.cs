#nullable enable

// Config สำหรับ combat SFX — player + enemy
// concrete impl คือ CombatSoundConfig (SO ใน 4_Infrastructure)
public interface ICombatSoundConfig
{
  // ---- Player ----
  SoundKey? OnPlayerHit { get; }
  SoundKey? OnPlayerDeath { get; }
  SoundKey? OnPlayerHeal { get; }

  // ---- Enemy (Phase 2.2) ----
  SoundKey? OnEnemyHit { get; }
  SoundKey? OnEnemyDeath { get; }
}
