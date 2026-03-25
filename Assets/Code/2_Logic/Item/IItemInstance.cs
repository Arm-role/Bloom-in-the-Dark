public interface IItemInstance
{
  IItemDefinition Data { get; }
  int Level { get; }
  ItemStatService Stats { get; }
}