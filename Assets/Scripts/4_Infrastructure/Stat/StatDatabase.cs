using UnityEngine;

[CreateAssetMenu(menuName = "Stat/StatDatabase")]
public class StatDatabase : ScriptableObject, IStatDatabase
{
  [SerializeField] private StatKey moveSpeed;
  [SerializeField] private StatKey maxHp;
  [SerializeField] private StatKey hpRefill;
  [SerializeField] private StatKey maxEnergy;
  [SerializeField] private StatKey energyRefill;
  [SerializeField] private StatKey cooldown;
  [SerializeField] private StatKey farmArea;

  public StatKey MoveSpeed => moveSpeed;
  public StatKey MaxHp => maxHp;
  public StatKey HpRefill => hpRefill;
  public StatKey MaxEnergy => maxEnergy;
  public StatKey EnergyRefill => energyRefill;
  public StatKey Cooldown => cooldown;
  public StatKey FarmArea => farmArea;
}