public readonly struct IncreaseEnergyCommand : IPlayerCommand
{
  public readonly float Amount;

  public IncreaseEnergyCommand(float amount)
  {
    Amount = amount;
  }
}