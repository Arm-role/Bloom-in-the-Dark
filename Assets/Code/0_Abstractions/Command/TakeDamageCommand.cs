public readonly struct TakeDamageCommand : IPlayerCommand
{
    public readonly float Amount;

    public TakeDamageCommand(float amount)
    {
        Amount = amount;
    }
}