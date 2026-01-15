public readonly struct ConsumeEnergyCommand : IPlayerCommand
{
    public readonly float Amount;

    public ConsumeEnergyCommand(float amount)
    {
        Amount = amount;
    }
}
public readonly struct IncreaseMaxEnergyCommand : IPlayerCommand
{
    public readonly float Amount;

    public IncreaseMaxEnergyCommand(float amount)
    {
        Amount = amount;
    }
}