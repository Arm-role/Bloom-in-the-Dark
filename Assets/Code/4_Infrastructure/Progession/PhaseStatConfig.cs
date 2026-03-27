using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade/PlayerStatConfig")]
public class PhaseStatConfig : ScriptableObject, IGameStatConfig
{
  [Header("GameTag")]
  [SerializeField] private GlobalKey key;

  [Header("StatKey")]
  [SerializeField] private StatKey moveSpeedKey;
  [SerializeField] private StatKey detectDistanceKey;
  [SerializeField] private StatKey maxHPKey;
  [SerializeField] private StatKey maxEnergyKey;
  [SerializeField] private StatKey refillKey;

  [Header("Value")]
  [SerializeField] private float moveSpeed;
  [SerializeField] private float detectDistance;
  [SerializeField] private int maxHP;
  [SerializeField] private int maxEnergy;
  [SerializeField] private float baseRefill;

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
        { moveSpeedKey, moveSpeed},
        { detectDistanceKey, detectDistance},
        { maxHPKey, maxHP},
        { maxEnergyKey, maxEnergy},
        { refillKey, baseRefill },
    };
  }

  public float GetBaseStat(StatKey key)
  {
    if (_baseStats == null)
      BuildStatMap();

    return _baseStats.TryGetValue(key, out var v) ? v : 0f;
  }
}