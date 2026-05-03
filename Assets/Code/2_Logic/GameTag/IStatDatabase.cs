public interface IStatDatabase
{
  StatKey MoveSpeed { get; }
  StatKey MaxEnergy { get; }
  StatKey EnergyRefill { get; }
  StatKey Cooldown { get; }
  StatKey HpRefill { get; }
  StatKey MaxHp { get; }
  StatKey FarmArea { get; }
}