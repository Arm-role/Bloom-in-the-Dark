public interface ICooldownOwner
{
    bool IsOnCooldown(string key);

    bool TryGetCooldown(
        string key,
        out CooldownData cooldown);

    void ApplyCooldown(
        string key,
        float duration);
}