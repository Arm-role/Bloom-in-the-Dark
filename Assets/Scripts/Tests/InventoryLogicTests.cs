#nullable enable

using NUnit.Framework;

public sealed class InventoryLogicTests
{
  // ===============================
  // TryAddItem — fill existing → empty slot, clones per new stack
  // ===============================

  [Test]
  public void TryAddItem_FillsExistingStackBeforeEmptySlot()
  {
    var inv = new InventoryLogic(3);
    var def = new FakeItemDefinition(1, "stone", maxStackSize: 64);
    inv.Slots[0].SetItem(new FakeItemInstance(def), 30);

    int leftover = inv.TryAddItem(new FakeItemInstance(def), 20);

    Assert.AreEqual(0, leftover);
    Assert.AreEqual(50, inv.Slots[0].Amount, "ควรเติม stack เดิมก่อน");
    Assert.IsTrue(inv.Slots[1].IsEmpty, "ยังไม่ใช้ empty slot");
  }

  [Test]
  public void TryAddItem_OverflowSpillsToEmptySlot()
  {
    var inv = new InventoryLogic(3);
    var def = new FakeItemDefinition(1, "stone", maxStackSize: 64);
    inv.Slots[0].SetItem(new FakeItemInstance(def), 60);

    int leftover = inv.TryAddItem(new FakeItemInstance(def), 20);

    Assert.AreEqual(0, leftover);
    Assert.AreEqual(64, inv.Slots[0].Amount, "เติมเต็ม");
    Assert.AreEqual(16, inv.Slots[1].Amount, "ที่เหลือไป empty slot");
  }

  [Test]
  public void TryAddItem_SplitsAcrossMultipleEmptySlots()
  {
    var inv = new InventoryLogic(3);
    var def = new FakeItemDefinition(1, "stone", maxStackSize: 64);

    int leftover = inv.TryAddItem(new FakeItemInstance(def), 200);

    Assert.AreEqual(8, leftover, "200 - (64*3) = 8 เกินทุก slot");
    Assert.AreEqual(64, inv.Slots[0].Amount);
    Assert.AreEqual(64, inv.Slots[1].Amount);
    Assert.AreEqual(64, inv.Slots[2].Amount);
  }

  [Test]
  public void TryAddItem_FullInventory_ReturnsAllAmount()
  {
    var inv = new InventoryLogic(2);
    var def = new FakeItemDefinition(1, "stone", maxStackSize: 64);
    inv.Slots[0].SetItem(new FakeItemInstance(def), 64);
    inv.Slots[1].SetItem(new FakeItemInstance(def), 64);

    int leftover = inv.TryAddItem(new FakeItemInstance(def), 50);

    Assert.AreEqual(50, leftover);
  }

  [Test]
  public void TryAddItem_EmptySlots_GetDistinctInstancesPerStack()
  {
    var inv = new InventoryLogic(2);
    var def = new FakeItemDefinition(1, "stone", maxStackSize: 64);
    var source = new FakeItemInstance(def);

    inv.TryAddItem(source, 100);

    var first = inv.Slots[0].GetItemInstance();
    var second = inv.Slots[1].GetItemInstance();
    Assert.IsNotNull(first);
    Assert.IsNotNull(second);
    Assert.AreNotSame(first, second, "ต้อง Clone ทุก new stack — กัน share state");
    Assert.AreNotSame(source, first, "ไม่ใช้ source instance ตรงๆ");
  }

  [Test]
  public void TryAddItem_FiresOnItemAddedPerOperation()
  {
    var inv = new InventoryLogic(2);
    var def = new FakeItemDefinition(1, "stone", maxStackSize: 64);
    int totalAdded = 0;
    inv.OnItemAdded += (_, n) => totalAdded += n;

    inv.TryAddItem(new FakeItemInstance(def), 100);

    Assert.AreEqual(100, totalAdded);
  }

  [Test]
  public void TryAddItem_DifferentDataDoesNotMergeIntoExistingStack()
  {
    var inv = new InventoryLogic(3);
    var stone = new FakeItemDefinition(1, "stone", maxStackSize: 64);
    var wood = new FakeItemDefinition(2, "wood", maxStackSize: 64);
    inv.Slots[0].SetItem(new FakeItemInstance(stone), 30);

    inv.TryAddItem(new FakeItemInstance(wood), 10);

    Assert.AreEqual(30, inv.Slots[0].Amount, "stone stack ไม่โดน wood แตะ");
    Assert.AreEqual(10, inv.Slots[1].Amount);
  }

  // ===============================
  // CanAddItem
  // ===============================

  [Test]
  public void CanAddItem_FitsInExistingSpace_True()
  {
    var inv = new InventoryLogic(2);
    var def = new FakeItemDefinition(1, "stone", maxStackSize: 64);
    inv.Slots[0].SetItem(new FakeItemInstance(def), 60);

    Assert.IsTrue(inv.CanAddItem(new FakeItemInstance(def), 4));
  }

  [Test]
  public void CanAddItem_NeedsEmptySlot_TrueWhenAvailable()
  {
    var inv = new InventoryLogic(2);
    var def = new FakeItemDefinition(1, "stone", maxStackSize: 64);
    inv.Slots[0].SetItem(new FakeItemInstance(def), 64);

    Assert.IsTrue(inv.CanAddItem(new FakeItemInstance(def), 30));
  }

  [Test]
  public void CanAddItem_OverCapacity_False()
  {
    var inv = new InventoryLogic(2);
    var def = new FakeItemDefinition(1, "stone", maxStackSize: 64);
    inv.Slots[0].SetItem(new FakeItemInstance(def), 64);
    inv.Slots[1].SetItem(new FakeItemInstance(def), 64);

    Assert.IsFalse(inv.CanAddItem(new FakeItemInstance(def), 1));
  }

  // ===============================
  // TryRemoveItem — front to back (slot 0 first)
  // ===============================

  [Test]
  public void TryRemoveItem_ConsumesFromSlotZeroFirst()
  {
    var inv = new InventoryLogic(3);
    var def = new FakeItemDefinition(1, "stone");
    inv.Slots[0].SetItem(new FakeItemInstance(def), 10);
    inv.Slots[2].SetItem(new FakeItemInstance(def), 10);

    int leftover = inv.TryRemoveItem(def, 5);

    Assert.AreEqual(0, leftover);
    Assert.AreEqual(5, inv.Slots[0].Amount, "slot 0 ลดก่อน");
    Assert.AreEqual(10, inv.Slots[2].Amount, "slot 2 ยังไม่แตะ");
  }

  [Test]
  public void TryRemoveItem_SpansMultipleSlotsFrontToBack()
  {
    var inv = new InventoryLogic(3);
    var def = new FakeItemDefinition(1, "stone");
    inv.Slots[0].SetItem(new FakeItemInstance(def), 5);
    inv.Slots[1].SetItem(new FakeItemInstance(def), 5);
    inv.Slots[2].SetItem(new FakeItemInstance(def), 5);

    int leftover = inv.TryRemoveItem(def, 12);

    Assert.AreEqual(0, leftover);
    Assert.IsTrue(inv.Slots[0].IsEmpty);
    Assert.IsTrue(inv.Slots[1].IsEmpty);
    Assert.AreEqual(3, inv.Slots[2].Amount);
  }

  [Test]
  public void TryRemoveItem_NotEnough_ReturnsRemainder()
  {
    var inv = new InventoryLogic(2);
    var def = new FakeItemDefinition(1, "stone");
    inv.Slots[0].SetItem(new FakeItemInstance(def), 3);

    int leftover = inv.TryRemoveItem(def, 10);

    Assert.AreEqual(7, leftover);
    Assert.IsTrue(inv.Slots[0].IsEmpty);
  }

  [Test]
  public void CountItem_SumsAllMatchingSlots()
  {
    var inv = new InventoryLogic(3);
    var stone = new FakeItemDefinition(1, "stone");
    var wood = new FakeItemDefinition(2, "wood");
    inv.Slots[0].SetItem(new FakeItemInstance(stone), 5);
    inv.Slots[1].SetItem(new FakeItemInstance(wood), 50);
    inv.Slots[2].SetItem(new FakeItemInstance(stone), 7);

    Assert.AreEqual(12, inv.CountItem(stone));
  }

  // ===============================
  // SwapSlots — content swap, no reference swap, both events fire
  // ===============================

  [Test]
  public void SwapSlots_SwapsContent_NotReferences()
  {
    var inv = new InventoryLogic(2);
    var stone = new FakeItemDefinition(1, "stone");
    var wood = new FakeItemDefinition(2, "wood");
    inv.Slots[0].SetItem(new FakeItemInstance(stone), 5);
    inv.Slots[1].SetItem(new FakeItemInstance(wood), 7);

    var refSlot0 = inv.Slots[0];
    var refSlot1 = inv.Slots[1];

    inv.SwapSlots(0, 1);

    Assert.AreSame(refSlot0, inv.Slots[0], "slot object คงเดิม (event subscriber ยัง bind ถูก)");
    Assert.AreSame(refSlot1, inv.Slots[1]);
    Assert.AreEqual(wood, inv.Slots[0].GetItemInstance()!.Data);
    Assert.AreEqual(stone, inv.Slots[1].GetItemInstance()!.Data);
    Assert.AreEqual(7, inv.Slots[0].Amount);
    Assert.AreEqual(5, inv.Slots[1].Amount);
  }

  [Test]
  public void SwapSlots_FiresOnSlotChangedOnBothSlots()
  {
    var inv = new InventoryLogic(2);
    var stone = new FakeItemDefinition(1, "stone");
    var wood = new FakeItemDefinition(2, "wood");
    inv.Slots[0].SetItem(new FakeItemInstance(stone), 5);
    inv.Slots[1].SetItem(new FakeItemInstance(wood), 7);

    int s0Fired = 0;
    int s1Fired = 0;
    inv.Slots[0].OnSlotChanged += _ => s0Fired++;
    inv.Slots[1].OnSlotChanged += _ => s1Fired++;

    inv.SwapSlots(0, 1);

    Assert.GreaterOrEqual(s0Fired, 1, "slot 0 view ต้อง refresh");
    Assert.GreaterOrEqual(s1Fired, 1, "slot 1 view ต้อง refresh");
  }

  [Test]
  public void SwapSlots_OneSideEmpty_HandlesNullItem()
  {
    var inv = new InventoryLogic(2);
    var stone = new FakeItemDefinition(1, "stone");
    inv.Slots[0].SetItem(new FakeItemInstance(stone), 5);

    inv.SwapSlots(0, 1);

    Assert.IsTrue(inv.Slots[0].IsEmpty);
    Assert.AreEqual(stone, inv.Slots[1].GetItemInstance()!.Data);
    Assert.AreEqual(5, inv.Slots[1].Amount);
  }

  [Test]
  public void SwapSlots_SameIndex_NoOp()
  {
    var inv = new InventoryLogic(2);
    var stone = new FakeItemDefinition(1, "stone");
    inv.Slots[0].SetItem(new FakeItemInstance(stone), 5);

    int fired = 0;
    inv.Slots[0].OnSlotChanged += _ => fired++;

    inv.SwapSlots(0, 0);

    Assert.AreEqual(0, fired, "swap กับตัวเอง ไม่ควรยิง event");
    Assert.AreEqual(5, inv.Slots[0].Amount);
  }
}
