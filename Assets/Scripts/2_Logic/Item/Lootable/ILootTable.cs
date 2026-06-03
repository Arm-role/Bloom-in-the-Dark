public interface ILootTable
{
  // Legacy entry — ไม่มี cap filter (เทียบเท่ากับ RollLoot(LootContext.None, toolUsed))
  // Phase 2+ จะ migrate caller มาใช้ overload ใหม่
  (int Exp, ItemStack[]) RollLoot(IItemDefinition toolUsed = null);

  // Filter-aware entry — ใช้ context.Census เช็ค GlobalCap ของแต่ละ LootDrop
  (int Exp, ItemStack[]) RollLoot(LootContext context, IItemDefinition toolUsed = null);
}
