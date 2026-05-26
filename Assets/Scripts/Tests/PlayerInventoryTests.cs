#nullable enable

using NUnit.Framework;

public sealed class PlayerInventoryTests
{
  // -----------------------------
  // Helpers
  // -----------------------------

  private static PlayerInventory MakeInventory(int hotbarSize = 3, int mainSize = 3)
  {
    var emptyDef = new FakeItemDefinition(0, "empty");
    return new PlayerInventory(
      new FakeItemInstance(emptyDef),
      new HotbarState(hotbarSize),
      new InventoryLogic(hotbarSize),
      new InventoryLogic(mainSize));
  }

  private static FakeItemDefinition Stone() => new(1, "stone", maxStackSize: 64);
  private static FakeItemDefinition Wood() => new(2, "wood", maxStackSize: 64);

  // ===============================
  // AddItem — hotbar before main
  // ===============================

  [Test]
  public void AddItem_FillsHotbarBeforeMain()
  {
    var inv = MakeInventory(hotbarSize: 1, mainSize: 2);
    var stone = Stone();

    int leftover = inv.AddItem(new FakeItemInstance(stone), 50);

    Assert.AreEqual(0, leftover);
    Assert.AreEqual(50, inv.Hotbar.Slots[0].Amount);
    Assert.IsTrue(inv.MainInventory.Slots[0].IsEmpty);
  }

  [Test]
  public void AddItem_OverflowSpillsToMain()
  {
    var inv = MakeInventory(hotbarSize: 1, mainSize: 2);
    var stone = Stone();

    int leftover = inv.AddItem(new FakeItemInstance(stone), 100);

    Assert.AreEqual(0, leftover);
    Assert.AreEqual(64, inv.Hotbar.Slots[0].Amount);
    Assert.AreEqual(36, inv.MainInventory.Slots[0].Amount);
  }

  // ===============================
  // TryPick
  // ===============================

  [Test]
  public void TryPick_EmptySlot_ReturnsFalse()
  {
    var inv = MakeInventory();

    bool ok = inv.TryPick(InventorySide.Hotbar, 0, out var item, out var amount);

    Assert.IsFalse(ok);
    Assert.IsNull(item);
    Assert.AreEqual(0, amount);
  }

  [Test]
  public void TryPick_NonEmpty_ClearsSourceAndReturnsItem()
  {
    var inv = MakeInventory();
    var stone = Stone();
    inv.Hotbar.Slots[0].SetItem(new FakeItemInstance(stone), 12);

    bool ok = inv.TryPick(InventorySide.Hotbar, 0, out var item, out var amount);

    Assert.IsTrue(ok);
    Assert.AreEqual(stone, item!.Data);
    Assert.AreEqual(12, amount);
    Assert.IsTrue(inv.Hotbar.Slots[0].IsEmpty, "source slot ต้องว่างหลัง Pick");
  }

  // ===============================
  // Place — same slot back
  // ===============================

  [Test]
  public void Place_BackToSameSlot_RestoresItem()
  {
    var inv = MakeInventory();
    var stone = Stone();
    var item = new FakeItemInstance(stone);

    inv.Place(InventorySide.Hotbar, 0, item, 5, InventorySide.Hotbar, 0);

    Assert.AreEqual(5, inv.Hotbar.Slots[0].Amount);
    Assert.AreSame(item, inv.Hotbar.Slots[0].GetItemInstance());
  }

  // ===============================
  // Place — empty target
  // ===============================

  [Test]
  public void Place_OnEmptyTarget_MovesEverything()
  {
    var inv = MakeInventory();
    var stone = Stone();
    var item = new FakeItemInstance(stone);

    inv.Place(InventorySide.Hotbar, 0, item, 8,
             sourceSide: InventorySide.Hotbar, sourceIndex: 1);

    Assert.AreEqual(8, inv.Hotbar.Slots[0].Amount);
    Assert.IsTrue(inv.Hotbar.Slots[1].IsEmpty);
  }

  // ===============================
  // Place — same-Data merge (Minecraft semantics)
  // ===============================

  [Test]
  public void Place_OnSameDataSlot_MergesUpToMaxStack()
  {
    var inv = MakeInventory();
    var stone = Stone();
    inv.Hotbar.Slots[0].SetItem(new FakeItemInstance(stone), 30);

    inv.Place(InventorySide.Hotbar, 0, new FakeItemInstance(stone), 20,
             sourceSide: InventorySide.Hotbar, sourceIndex: 1);

    Assert.AreEqual(50, inv.Hotbar.Slots[0].Amount, "ควร merge เป็น 50 ไม่ใช่ swap");
    Assert.IsTrue(inv.Hotbar.Slots[1].IsEmpty, "source ไม่มี overflow → ยังว่าง");
  }

  [Test]
  public void Place_OnSameDataSlot_OverflowReturnsToSourceSlot()
  {
    var inv = MakeInventory();
    var stone = Stone();
    inv.Hotbar.Slots[0].SetItem(new FakeItemInstance(stone), 60);
    // source slot empty (จำลองหลัง TryPick) — Place จะคืน overflow ที่นี่

    inv.Place(InventorySide.Hotbar, 0, new FakeItemInstance(stone), 20,
             sourceSide: InventorySide.Hotbar, sourceIndex: 1);

    Assert.AreEqual(64, inv.Hotbar.Slots[0].Amount, "target เต็มที่ MaxStack");
    Assert.AreEqual(16, inv.Hotbar.Slots[1].Amount, "overflow 16 คืน source");
  }

  [Test]
  public void Place_OnSameDataFullSlot_ReturnsToSourceNoSwap()
  {
    var inv = MakeInventory();
    var stone = Stone();
    inv.Hotbar.Slots[0].SetItem(new FakeItemInstance(stone), 64);
    // ที่ slot 0 เต็มแล้ว — Place ของ stone อีก 10 → คืน source

    inv.Place(InventorySide.Hotbar, 0, new FakeItemInstance(stone), 10,
             sourceSide: InventorySide.Hotbar, sourceIndex: 1);

    Assert.AreEqual(64, inv.Hotbar.Slots[0].Amount, "ไม่แตะ target ที่เต็ม");
    Assert.AreEqual(10, inv.Hotbar.Slots[1].Amount, "คืน source slot");
    Assert.AreEqual(stone, inv.Hotbar.Slots[1].GetItemInstance()!.Data);
  }

  // ===============================
  // Place — different-Data swap
  // ===============================

  [Test]
  public void Place_OnDifferentDataSlot_Swaps()
  {
    var inv = MakeInventory();
    var stone = Stone();
    var wood = Wood();
    inv.Hotbar.Slots[0].SetItem(new FakeItemInstance(stone), 5);

    inv.Place(InventorySide.Hotbar, 0, new FakeItemInstance(wood), 10,
             sourceSide: InventorySide.Hotbar, sourceIndex: 1);

    Assert.AreEqual(wood, inv.Hotbar.Slots[0].GetItemInstance()!.Data);
    Assert.AreEqual(10, inv.Hotbar.Slots[0].Amount);
    Assert.AreEqual(stone, inv.Hotbar.Slots[1].GetItemInstance()!.Data);
    Assert.AreEqual(5, inv.Hotbar.Slots[1].Amount);
  }

  // ===============================
  // QuickMove — merge then empty, partial leftover
  // ===============================

  [Test]
  public void QuickMove_MergesIntoExistingStackInOppositeSide()
  {
    var inv = MakeInventory();
    var stone = Stone();
    inv.Hotbar.Slots[0].SetItem(new FakeItemInstance(stone), 20);
    inv.MainInventory.Slots[0].SetItem(new FakeItemInstance(stone), 10);

    bool ok = inv.QuickMove(InventorySide.Hotbar, 0);

    Assert.IsTrue(ok);
    Assert.IsTrue(inv.Hotbar.Slots[0].IsEmpty);
    Assert.AreEqual(30, inv.MainInventory.Slots[0].Amount, "merge stack เดิมก่อน");
  }

  [Test]
  public void QuickMove_PartialMove_LeavesLeftoverInSource()
  {
    var inv = MakeInventory(hotbarSize: 1, mainSize: 1);
    var stone = Stone();
    inv.Hotbar.Slots[0].SetItem(new FakeItemInstance(stone), 50);
    inv.MainInventory.Slots[0].SetItem(new FakeItemInstance(stone), 60);
    // dst เติมได้แค่ 4 → src เหลือ 46

    bool ok = inv.QuickMove(InventorySide.Hotbar, 0);

    Assert.IsTrue(ok);
    Assert.AreEqual(46, inv.Hotbar.Slots[0].Amount);
    Assert.AreEqual(64, inv.MainInventory.Slots[0].Amount);
  }

  [Test]
  public void QuickMove_NoSpaceAtAll_ReturnsFalse()
  {
    var inv = MakeInventory(hotbarSize: 1, mainSize: 1);
    var stone = Stone();
    var wood = Wood();
    inv.Hotbar.Slots[0].SetItem(new FakeItemInstance(stone), 5);
    inv.MainInventory.Slots[0].SetItem(new FakeItemInstance(wood), 64);
    // dst ไม่มีช่อง เพราะมี wood เต็ม, ของก็คนละ data

    bool ok = inv.QuickMove(InventorySide.Hotbar, 0);

    Assert.IsFalse(ok);
    Assert.AreEqual(5, inv.Hotbar.Slots[0].Amount, "ของไม่ขยับ");
  }

  [Test]
  public void QuickMove_EmptySource_ReturnsFalse()
  {
    var inv = MakeInventory();

    Assert.IsFalse(inv.QuickMove(InventorySide.Hotbar, 0));
  }

  // ===============================
  // MoveFromInventoryToHotbar / MoveHotbarToInventory
  // ===============================

  [Test]
  public void MoveFromInventoryToHotbar_EmptyHotbarSlot_Moves()
  {
    var inv = MakeInventory();
    var stone = Stone();
    inv.MainInventory.Slots[0].SetItem(new FakeItemInstance(stone), 7);

    bool ok = inv.MoveFromInventoryToHotbar(0, 0);

    Assert.IsTrue(ok);
    Assert.IsTrue(inv.MainInventory.Slots[0].IsEmpty);
    Assert.AreEqual(7, inv.Hotbar.Slots[0].Amount);
  }

  [Test]
  public void MoveFromInventoryToHotbar_SameData_Merges()
  {
    var inv = MakeInventory();
    var stone = Stone();
    inv.MainInventory.Slots[0].SetItem(new FakeItemInstance(stone), 30);
    inv.Hotbar.Slots[0].SetItem(new FakeItemInstance(stone), 20);

    bool ok = inv.MoveFromInventoryToHotbar(0, 0);

    Assert.IsTrue(ok);
    Assert.AreEqual(50, inv.Hotbar.Slots[0].Amount);
    Assert.IsTrue(inv.MainInventory.Slots[0].IsEmpty);
  }

  [Test]
  public void MoveFromInventoryToHotbar_SameData_OverflowStaysInSource()
  {
    var inv = MakeInventory();
    var stone = Stone();
    inv.MainInventory.Slots[0].SetItem(new FakeItemInstance(stone), 50);
    inv.Hotbar.Slots[0].SetItem(new FakeItemInstance(stone), 60);

    bool ok = inv.MoveFromInventoryToHotbar(0, 0);

    Assert.IsTrue(ok);
    Assert.AreEqual(64, inv.Hotbar.Slots[0].Amount);
    Assert.AreEqual(46, inv.MainInventory.Slots[0].Amount);
  }

  [Test]
  public void MoveFromInventoryToHotbar_DifferentData_Swaps()
  {
    var inv = MakeInventory();
    var stone = Stone();
    var wood = Wood();
    inv.MainInventory.Slots[0].SetItem(new FakeItemInstance(stone), 5);
    inv.Hotbar.Slots[0].SetItem(new FakeItemInstance(wood), 7);

    bool ok = inv.MoveFromInventoryToHotbar(0, 0);

    Assert.IsTrue(ok);
    Assert.AreEqual(stone, inv.Hotbar.Slots[0].GetItemInstance()!.Data);
    Assert.AreEqual(5, inv.Hotbar.Slots[0].Amount);
    Assert.AreEqual(wood, inv.MainInventory.Slots[0].GetItemInstance()!.Data);
    Assert.AreEqual(7, inv.MainInventory.Slots[0].Amount);
  }

  [Test]
  public void MoveHotbarToInventory_MirrorBehavior()
  {
    var inv = MakeInventory();
    var stone = Stone();
    inv.Hotbar.Slots[0].SetItem(new FakeItemInstance(stone), 9);

    bool ok = inv.MoveHotbarToInventory(0, 0);

    Assert.IsTrue(ok);
    Assert.IsTrue(inv.Hotbar.Slots[0].IsEmpty);
    Assert.AreEqual(9, inv.MainInventory.Slots[0].Amount);
  }

  [Test]
  public void MoveFromInventoryToHotbar_InvalidIndex_ReturnsFalse()
  {
    var inv = MakeInventory(hotbarSize: 2, mainSize: 2);

    Assert.IsFalse(inv.MoveFromInventoryToHotbar(-1, 0));
    Assert.IsFalse(inv.MoveFromInventoryToHotbar(0, 5));
  }

  // ===============================
  // RemoveItemAnywhere
  // ===============================

  [Test]
  public void RemoveItemAnywhere_DrainsHotbarThenMain()
  {
    var inv = MakeInventory();
    var stone = Stone();
    inv.Hotbar.Slots[0].SetItem(new FakeItemInstance(stone), 5);
    inv.MainInventory.Slots[0].SetItem(new FakeItemInstance(stone), 5);

    int leftover = inv.RemoveItemAnywhere(stone, 7);

    Assert.AreEqual(0, leftover);
    Assert.IsTrue(inv.Hotbar.Slots[0].IsEmpty);
    Assert.AreEqual(3, inv.MainInventory.Slots[0].Amount);
  }

  [Test]
  public void CountItemAnywhere_SumsBoth()
  {
    var inv = MakeInventory();
    var stone = Stone();
    inv.Hotbar.Slots[0].SetItem(new FakeItemInstance(stone), 5);
    inv.MainInventory.Slots[0].SetItem(new FakeItemInstance(stone), 8);

    Assert.AreEqual(13, inv.CountItemAnywhere(stone));
  }

  // ===============================
  // GetHotbarSlotSelected
  // ===============================

  [Test]
  public void GetHotbarSlotSelected_ReturnsCurrentHotbarStateSlot()
  {
    var inv = MakeInventory(hotbarSize: 3);
    inv.HotbarState.SelectSlot(2);

    Assert.AreSame(inv.Hotbar.Slots[2], inv.GetHotbarSlotSelected());
  }
}
