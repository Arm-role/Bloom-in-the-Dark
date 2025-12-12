public class CooldownCondition : IActionCondition
{
    public CooldownCondition()
    {
    }

    public bool Check(IDataProvider data, InteractionHandleContext ctx, out string reason)
    {
        reason = "Skill on cooldown";
        return false;
    }
}