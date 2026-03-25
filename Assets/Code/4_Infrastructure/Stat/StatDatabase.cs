using UnityEngine;

[CreateAssetMenu(menuName = "Stat/StatDatabase")]
public class StatDatabase : ScriptableObject, IStatDatabase
{
  [SerializeField] private StatKey moveSpeed;
  [SerializeField] private StatKey maxEnergy;
  [SerializeField] private StatKey energyRefill;
  [SerializeField] private StatKey cooldown;

  public StatKey MoveSpeed => moveSpeed;
  public StatKey MaxEnergy => maxEnergy;
  public StatKey EnergyRefill => energyRefill;
  public StatKey Cooldown => cooldown;
}