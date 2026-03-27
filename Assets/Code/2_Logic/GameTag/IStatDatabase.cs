public interface IStatDatabase
{
  StatKey MoveSpeed { get; }
  StatKey MaxEnergy { get; }
  StatKey EnergyRefill { get; }
  StatKey Cooldown { get; }
}