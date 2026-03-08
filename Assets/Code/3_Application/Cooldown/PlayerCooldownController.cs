public class PlayerCooldownController : ICooldownOwner
{
  private readonly CooldownContainer _container;
  public PlayerCooldownController(CooldownContainer container)
  {
    _container = container;
  }

  public bool IsOnCooldown(string key)
    => _container.IsOnCooldown(key);

  public bool TryApply(string key, float duration)
    => _container.TryApply(key, duration);

  public void ApplyCooldown(string key, float duration)
    => _container.TryApply(key, duration);

  public bool TryGetCooldown(string key, out CooldownData cooldown)
    => _container.TryGetCooldown(key, out cooldown);
}
