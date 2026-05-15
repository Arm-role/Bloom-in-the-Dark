public static class InteractionConditionExtension
{
    public static bool IsMet(
        this InteractionCondition condition,
        InteractionConditionContext ctx)
    {
        if (condition == InteractionCondition.None)
            return true;

        if (condition.HasFlag(InteractionCondition.RequireSecondaryHeld) &&
            !ctx.Held.HasFlag(InputActionType.Secondary))
            return false;

        if (condition.HasFlag(InteractionCondition.RequireNoSecondaryHeld) &&
            ctx.Held.HasFlag(InputActionType.Secondary))
            return false;

        if (condition.HasFlag(InteractionCondition.RequirePrimaryHeld) &&
            !ctx.Held.HasFlag(InputActionType.Primary))
            return false;

        if (condition.HasFlag(InteractionCondition.RequireNoSkillPreviewActive) &&
            ctx.IsSkillPreviewActive)
            return false;

        return true;
    }
}