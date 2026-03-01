public interface IInteractionFeedbackResolver
{
    InteractionFeedback Resolve(
        InteractionResult result,
        InteractionIntent intent);
}