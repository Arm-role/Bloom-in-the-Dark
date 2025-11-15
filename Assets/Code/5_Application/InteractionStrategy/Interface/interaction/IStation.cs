public interface IStation : IWorldInteractable
{
    bool IsBusy { get; }
    void StartProcess(InteractionHandleContext context);
    void StopProcess();
}