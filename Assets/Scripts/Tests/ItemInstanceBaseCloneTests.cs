#nullable enable

using NUnit.Framework;

public sealed class ItemInstanceBaseCloneTests
{
  // ItemInstanceBase ใช้ ItemStatService จริง — ต้องส่ง stub IStatDatabase + IUpgradeContainer
  // Stats จะถูก construct แต่ไม่ถูกอ่านใน test เลย (Clone ไม่แตะ Stats internals)
  private static ItemInstanceBase MakeItem(int level = 1, int maxStack = 64)
  {
    var def = new FakeItemDefinition(1, "stone", maxStack);
    return new ItemInstanceBase(def, new FakeStatDatabase(), new FakeUpgradeContainer(), level);
  }

  [Test]
  public void Clone_ReturnsDifferentInstance()
  {
    var original = MakeItem();

    var clone = original.Clone();

    Assert.AreNotSame(original, clone, "Clone ต้องเป็น instance ใหม่");
  }

  [Test]
  public void Clone_PreservesData()
  {
    var original = MakeItem();

    var clone = original.Clone();

    Assert.AreSame(original.Data, clone.Data, "Data (IItemDefinition) shared ได้ — เป็น SO/config ไม่ mutate");
  }

  [Test]
  public void Clone_PreservesLevel()
  {
    var original = MakeItem(level: 5);

    var clone = original.Clone();

    Assert.AreEqual(5, clone.Level);
  }

  [Test]
  public void Clone_StatsIsSeparateInstance()
  {
    var original = MakeItem();

    var clone = original.Clone();

    Assert.AreNotSame(original.Stats, clone.Stats,
      "Stats ต้องเป็น instance ใหม่ — กัน per-stack state share (event subscription, future preview cache)");
  }

  [Test]
  public void Clone_OfClone_StillIndependent()
  {
    var original = MakeItem(level: 3);

    var c1 = original.Clone();
    var c2 = c1.Clone();

    Assert.AreNotSame(c1, c2);
    Assert.AreNotSame(c1.Stats, c2.Stats);
    Assert.AreEqual(3, c2.Level);
  }

  [Test]
  public void Clone_OriginalAddLevel_DoesNotAffectClone()
  {
    var original = MakeItem(level: 1);
    var clone = (ItemInstanceBase)original.Clone();

    original.AddLevel(5);

    Assert.AreEqual(6, original.Level);
    Assert.AreEqual(1, clone.Level, "clone Level ต้องไม่ตามต้นฉบับ (proves per-instance isolation)");
  }
}
