public interface IStatDatabase
{
  StatKey Damage { get; }
  StatKey Radius { get; }
  StatKey Range { get; }
  StatKey MoveSpeed { get; }
  StatKey MaxEnergy { get; }
  StatKey EnergyRefill { get; }
  StatKey Cooldown { get; }
  StatKey HpRefill { get; }
  StatKey MaxHp { get; }
  StatKey FarmArea { get; }
}