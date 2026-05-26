#nullable enable

using UnityEngine;

// Global SFX สำหรับ world interaction — pickup/break/tile
// designer ใส่เฉพาะที่ต้องการเสียง — field null = silent
[CreateAssetMenu(menuName = "Audio/Interaction Sound Config")]
public sealed class InteractionSoundConfig : ScriptableObject, IInteractionSoundConfig
{
  [Header("Reward / Pickup")]
  [SerializeField] private SoundKey? _onPickup;

  [Header("Destructible (tree, plant, ore)")]
  [SerializeField] private SoundKey? _onDestructibleBreak;

  [Header("Tile change")]
  [SerializeField] private SoundKey? _onTilePlace;
  [SerializeField] private SoundKey? _onTileRemove;

  public SoundKey? OnPickup => _onPickup;
  public SoundKey? OnDestructibleBreak => _onDestructibleBreak;
  public SoundKey? OnTilePlace => _onTilePlace;
  public SoundKey? OnTileRemove => _onTileRemove;
}
