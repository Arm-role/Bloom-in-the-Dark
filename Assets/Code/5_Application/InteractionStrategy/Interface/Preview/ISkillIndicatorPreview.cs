public interface ISkillIndicatorPreview
{
    void Setup(InteractionHandleContext context);
    void EnablePreview(InteractionHandleContext context);
    void UpdatePreview(InteractionHandleContext context);
    void DisablePreview();
}