#nullable enable

using System;
using System.Collections.Generic;

// Shared fakes สำหรับ inventory tests — เก็บ minimal surface พอใช้กับ test
// ค่าที่ไม่เกี่ยวกับ inventory (Skill, Tags ฯลฯ) คืน null!/default พอผ่าน compiler
// ถ้า test ดันไปแตะ field พวกนั้นจะ crash ทันที (เห็นได้ชัด)
public sealed class FakeItemDefinition : IItemDefinition
{
  public int ID { get; }
  public string Name { get; }
  public int MaxStackSize { get; }

  public FakeItemDefinition(int id, string name, int maxStackSize = 64)
  {
    ID = id;
    Name = name;
    MaxStackSize = maxStackSize;
  }

  public string DisplayName => Name;
  public GameTag Key => default;
  public GameTagContainer Tags => null!;
  public EItemRole Role => default;
  public ISkillDefinition Skill => null!;
  public IPlacementProfile PlacementProfile => null!;
  public IItemInteractionProfile InteractionProfile => null!;
  public IItemInteractionCapability InteractionCapability => null!;

  public GameTagContainer CreateTagContainer() => null!;
  public bool HasTag(GameTag tag) => false;
}

public sealed class FakeItemInstance : IItemInstance
{
  public IItemDefinition Data { get; }
  public int Level { get; }
  public ItemStatService Stats { get; } = null!;

  public FakeItemInstance(IItemDefinition data, int level = 1)
  {
    Data = data;
    Level = level;
  }

  public IItemInstance Clone()
    => new FakeItemInstance(Data, Level);
}

public sealed class FakeStatDatabase : IStatDatabase
{
  public StatKey Damage => null!;
  public StatKey Radius => null!;
  public StatKey Range => null!;
  public StatKey MoveSpeed => null!;
  public StatKey MaxEnergy => null!;
  public StatKey EnergyRefill => null!;
  public StatKey Cooldown => null!;
  public StatKey HpRefill => null!;
  public StatKey MaxHp => null!;
  public StatKey FarmArea => null!;
}

public sealed class FakeUpgradeContainer : IUpgradeContainer
{
  public event Action<GameTag, StatKey>? onUpgrade;

  public IEnumerable<StatModifier> GetUpgrades(GameTag key)
    => Array.Empty<StatModifier>();

  // เก็บ raise method ไว้กัน "unused event" warning + เผื่อ test ในอนาคต
  internal void RaiseUpgrade(GameTag key, StatKey stat) => onUpgrade?.Invoke(key, stat);
}
