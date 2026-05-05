
public interface IGameStatConfig
{
  GameTag Key { get; }
  int LevelStart { get; }

  float GetBaseStat(StatKey key);
}