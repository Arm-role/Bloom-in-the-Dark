public class CooldownService
{
    private readonly PlayerCooldownController _playerCooldown;

    public CooldownService(PlayerCooldownController playerCooldown)
    {
        _playerCooldown = playerCooldown;
    }

    public bool CanExecute(
        InteractionHandleContext ctx,
        InteractionFeedback feedback)
    {
        var key = feedback.IntentType.ToString();
        return !_playerCooldown.IsOnCooldown(key);
    }

    public void Apply(
        InteractionHandleContext ctx,
        InteractionFeedback feedback)
    {
        if (!feedback.HasCost)
            return;

        var key = feedback.IntentType.ToString();
        _playerCooldown.ApplyCooldown(key, feedback.PlayerCooldown);
    }
}