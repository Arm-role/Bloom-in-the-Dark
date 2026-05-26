#nullable enable

using System;
using NUnit.Framework;

public sealed class InventorySlotTests
{
  // -----------------------------
  // SetItem — overflow throws (was silent truncate)
  // -----------------------------

  [Test]
  public void SetItem_AmountExceedsMaxStack_Throws()
  {
    var slot = new InventorySlot();
    var def = new FakeItemDefinition(1, "stone", maxStackSize: 64);
    var item = new FakeItemInstance(def);

    Assert.Throws<ArgumentOutOfRangeException>(
      () => slot.SetItem(item, 65));
  }

  [Test]
  public void SetItem_AmountEqualsMaxStack_OK()
  {
    var slot = new InventorySlot();
    var def = new FakeItemDefinition(1, "stone", maxStackSize: 64);
    var item = new FakeItemInstance(def);

    slot.SetItem(item, 64);

    Assert.AreEqual(64, slot.Amount);
    Assert.IsTrue(slot.IsFull);
  }

  [Test]
  public void SetItem_NullItem_ClearsSlot()
  {
    var slot = new InventorySlot();
    var def = new FakeItemDefinition(1, "stone");
    slot.SetItem(new FakeItemInstance(def), 5);

    slot.SetItem(null, 5);

    Assert.IsTrue(slot.IsEmpty);
    Assert.AreEqual(0, slot.Amount);
  }

  [Test]
  public void SetItem_ZeroAmount_ClearsSlot()
  {
    var slot = new InventorySlot();
    var def = new FakeItemDefinition(1, "stone");
    slot.SetItem(new FakeItemInstance(def), 5);

    slot.SetItem(new FakeItemInstance(def), 0);

    Assert.IsTrue(slot.IsEmpty);
  }

  // -----------------------------
  // AddAmount — clamps at MaxStack, fires event only if not empty
  // -----------------------------

  [Test]
  public void AddAmount_ClampsAtMaxStack()
  {
    var slot = new InventorySlot();
    var def = new FakeItemDefinition(1, "stone", maxStackSize: 64);
    slot.SetItem(new FakeItemInstance(def), 60);

    slot.AddAmount(20);

    Assert.AreEqual(64, slot.Amount);
  }

  [Test]
  public void AddAmount_OnEmptySlot_NoOp()
  {
    var slot = new InventorySlot();
    int eventFired = 0;
    slot.OnSlotChanged += _ => eventFired++;

    slot.AddAmount(5);

    Assert.AreEqual(0, slot.Amount);
    Assert.AreEqual(0, eventFired);
  }

  // -----------------------------
  // RemoveAmount — clears at 0
  // -----------------------------

  [Test]
  public void RemoveAmount_ToZero_ClearsSlot()
  {
    var slot = new InventorySlot();
    var def = new FakeItemDefinition(1, "stone");
    slot.SetItem(new FakeItemInstance(def), 5);

    slot.RemoveAmount(5);

    Assert.IsTrue(slot.IsEmpty);
  }

  [Test]
  public void RemoveAmount_BelowZero_ClearsSlot()
  {
    var slot = new InventorySlot();
    var def = new FakeItemDefinition(1, "stone");
    slot.SetItem(new FakeItemInstance(def), 5);

    slot.RemoveAmount(10);

    Assert.IsTrue(slot.IsEmpty);
  }

  [Test]
  public void RemoveAmount_OnEmptySlot_NoOp()
  {
    var slot = new InventorySlot();
    int eventFired = 0;
    slot.OnSlotChanged += _ => eventFired++;

    slot.RemoveAmount(5);

    Assert.AreEqual(0, eventFired);
  }

  // -----------------------------
  // OnSlotChanged event
  // -----------------------------

  [Test]
  public void SetItem_FiresOnSlotChangedOnce()
  {
    var slot = new InventorySlot();
    var def = new FakeItemDefinition(1, "stone");
    int fired = 0;
    slot.OnSlotChanged += _ => fired++;

    slot.SetItem(new FakeItemInstance(def), 5);

    Assert.AreEqual(1, fired);
  }

  [Test]
  public void Clear_FiresOnSlotChanged()
  {
    var slot = new InventorySlot();
    var def = new FakeItemDefinition(1, "stone");
    slot.SetItem(new FakeItemInstance(def), 5);

    int fired = 0;
    slot.OnSlotChanged += _ => fired++;

    slot.Clear();

    Assert.AreEqual(1, fired);
  }
}
