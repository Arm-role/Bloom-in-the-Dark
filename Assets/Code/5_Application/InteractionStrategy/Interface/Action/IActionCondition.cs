public interface IActionCondition
{
    bool Check(IDataProvider data, InteractionHandleContext ctx, out string reason);
}