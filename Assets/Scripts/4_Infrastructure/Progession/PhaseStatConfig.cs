using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade/PlayerStatConfig")]
public class PhaseStatConfig : ScriptableObject, IGameStatConfig
{
  [Header("GameTag")]
  [SerializeField] private GlobalKey key;

  [Header("StatKey")]
  [SerializeField] private StatKey baseRadiusKey;
  [SerializeField] private StatKey moveSpeedKey;
  [SerializeField] private StatKey detectDistanceKey;
  [SerializeField] private StatKey maxHPKey;
  [SerializeField] private StatKey hpRefillKey;
  [SerializeField] private StatKey maxEnergyKey;
  [SerializeField] private StatKey enegyRefillKey;

  [Header("Value")]
  [SerializeField] private float baseRadius;
  [SerializeField] private float moveSpeed;
  [SerializeField] private float detectDistance;
  [SerializeField] private int maxHP;
  [SerializeField] private float hpRefill;
  [SerializeField] private int maxEnergy;
  [SerializeField] private float enegyRefill;

  [Header("Static Value")]
  [SerializeField] private int levelStart;

  private Dictionary<StatKey, float> _baseStats;
  public GameTag Key => key.RuntimeTag;


  public int LevelStart => levelStart;
  private void OnEnable()
  {
    BuildStatMap();
  }

  private void BuildStatMap()
  {
    _baseStats = new Dictionary<StatKey, float>
    {
        { baseRadiusKey, baseRadius},
        { moveSpeedKey, moveSpeed},
        { detectDistanceKey, detectDistance},
        { maxHPKey, maxHP},
        { hpRefillKey, hpRefill},
        { maxEnergyKey, maxEnergy},
        { enegyRefillKey, enegyRefill},
    };
  }

  public float GetBaseStat(StatKey key)
  {
    if (_baseStats == null)
      BuildStatMap();

    return _baseStats.TryGetValue(key, out var v) ? v : 0f;
  }
}