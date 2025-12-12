public class EnergyCondition : IActionCondition
{
    private readonly PlayerState _player;
    private readonly int _cost;

    public EnergyCondition(PlayerState player, int cost)
    {
        _player = player;
        _cost = cost;
    }

    public bool Check(IDataProvider data, InteractionHandleContext ctx, out string reason)
    {

        reason = "Not enough energy";
        return false;
    }
}
