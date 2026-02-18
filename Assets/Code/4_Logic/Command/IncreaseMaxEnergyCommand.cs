public readonly struct IncreaseMaxEnergyCommand : IPlayerCommand
{
  public readonly float Amount;

  public IncreaseMaxEnergyCommand(float amount)
  {
    Amount = amount;
  }
}