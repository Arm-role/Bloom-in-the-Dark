#nullable enable

using UnityEngine;

// Combat SFX (player hit/death/heal + enemy hit/death)
// designer ใส่เฉพาะ key ที่ต้องการเสียง — field null = silent
[CreateAssetMenu(menuName = "Audio/Combat Sound Config")]
public sealed class CombatSoundConfig : ScriptableObject, ICombatSoundConfig
{
  [Header("Player")]
  [SerializeField] private SoundKey? _onPlayerHit;
  [SerializeField] private SoundKey? _onPlayerDeath;
  [SerializeField] private SoundKey? _onPlayerHeal;

  [Header("Enemy (shared — Phase 2.2 อาจ override ด้วย EnemyConfig per-enemy)")]
  [SerializeField] private SoundKey? _onEnemyHit;
  [SerializeField] private SoundKey? _onEnemyDeath;

  public SoundKey? OnPlayerHit => _onPlayerHit;
  public SoundKey? OnPlayerDeath => _onPlayerDeath;
  public SoundKey? OnPlayerHeal => _onPlayerHeal;
  public SoundKey? OnEnemyHit => _onEnemyHit;
  public SoundKey? OnEnemyDeath => _onEnemyDeath;
}
