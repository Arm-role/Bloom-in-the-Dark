public interface ICooldownOwner
{
  bool IsOnCooldown(string key);

  bool TryGetCooldown(
      string key,
      out CooldownData cooldown);

  bool TryApply(string cooldownKey, float cooldownDuration);
}