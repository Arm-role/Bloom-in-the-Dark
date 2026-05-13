public interface IGameActionResolver<T1>
{
  void Resolve(T1 param1, ActionRegistry registry);
}

public interface ICellActionResolver : IGameActionResolver<WorldCell> { }