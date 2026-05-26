#nullable enable

public interface ISkillDefinition
{
  int SkillId { get; }
  float GetBaseStat(StatKey key);
  bool Execute(IItemInstance instance, out ISkillDataPayload payload);

  // SFX ตอน cast (swing/projectile launch) — null = silent
  SoundKey? CastSfx { get; }
}