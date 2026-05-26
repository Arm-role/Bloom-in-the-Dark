#nullable enable

public interface IItemInstance
{
  IItemDefinition Data { get; }
  int Level { get; }
  ItemStatService Stats { get; }

  // Deep-copy พอที่จะใช้เป็น stack ใหม่อิสระ (Level + ของอื่นที่ mutate ได้)
  // ต้องไม่ share state กับต้นฉบับ — สำคัญสำหรับ InventoryLogic.TryAddItem ที่ split stack
  IItemInstance Clone();
}
