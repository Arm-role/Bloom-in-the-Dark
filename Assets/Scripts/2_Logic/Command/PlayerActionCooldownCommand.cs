public readonly struct PlayerActionCooldownCommand : IPlayerCommand
{
  public readonly object Owner;
  public readonly string CooldownGroup;
  public readonly float CooldownDuration;

  public PlayerActionCooldownCommand(object owner, string cooldownGroup, float cooldownDuration)
  {
    Owner = owner;
    CooldownGroup = cooldownGroup;
    CooldownDuration = cooldownDuration;
  }
}