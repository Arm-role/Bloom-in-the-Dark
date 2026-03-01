public readonly struct ConsumeEnergyCommand : IPlayerCommand
{
    public readonly float Amount;

    public ConsumeEnergyCommand(float amount)
    {
        Amount = amount;
    }
}