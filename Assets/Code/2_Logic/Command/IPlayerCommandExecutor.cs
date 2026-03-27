public interface IPlayerCommandExecutor
{
    bool CanExecute(IPlayerCommand command);
    bool TryExecute(IPlayerCommand command);
}