public struct AuxiliaryInput
{
    public InputActionType ActiveActions;
    public InputActionType ExecuteActions;
    public InputActionType ReleasedActions;

    public AuxiliaryInput(InputActionType activeActions, InputActionType executeActions, InputActionType releasedActions)
    {
        ActiveActions = activeActions;
        ExecuteActions = executeActions;
        ReleasedActions = releasedActions;
    }
}