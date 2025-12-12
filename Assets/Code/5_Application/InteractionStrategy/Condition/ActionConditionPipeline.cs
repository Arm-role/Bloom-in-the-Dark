using System.Collections.Generic;

public class ActionConditionPipeline
{
    private readonly List<IActionCondition> _conditions = new();

    public void Add(IActionCondition cond) => _conditions.Add(cond);

    public ValidationResult Validate(IDataProvider data, InteractionHandleContext ctx)
    {
        foreach (var cond in _conditions)
        {
            if (!cond.Check(data, ctx, out string reason))
            {
                return ValidationResult.Fail(reason);
            }
        }
        return ValidationResult.Success();
    }
}
