using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/BossConfig")]
public class BossConfig : EnemyConfig
{
  [Header("Boss Skills")]
  public List<SkillDefinitionSO> bossSkills = new();

  [Header("Boss Phase")]
  [Range(0f, 1f)] public float enrageHPPercent = 0.5f;
}
