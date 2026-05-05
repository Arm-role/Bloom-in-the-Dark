public static class InteractionConditionExtension
{
    public static bool IsMet(
        this InteractionCondition condition,
        InteractionConditionContext ctx)
    {
        switch (condition)
        {
            case InteractionCondition.None:
                return true;

            case InteractionCondition.RequireSecondaryHeld:
                return ctx.Held.HasFlag(InputActionType.Secondary);

            case InteractionCondition.RequireNoSecondaryHeld:
                return !ctx.Held.HasFlag(InputActionType.Secondary);

            case InteractionCondition.RequirePrimaryHeld:
                return ctx.Held.HasFlag(InputActionType.Primary);

            case InteractionCondition.RequireNoSkillPreviewActive:
                return !ctx.IsSkillPreviewActive;

            default:
                return false;
        }
    }
}