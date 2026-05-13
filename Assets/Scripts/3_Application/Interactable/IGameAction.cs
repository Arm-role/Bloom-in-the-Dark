public interface IGameAction
{
  InteractionStage Stage { get; }
  ETargetType TargetType { get; }
}