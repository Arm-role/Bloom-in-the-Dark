using UnityEngine;

[CreateAssetMenu(menuName = "AI/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
  [Header("Stats")]
  public int hp = 30;

  [Header("Skills")]
  public MeleeSkillDefinitionSO meleeSkill;      
  public SkillDefinitionSO uniqueSkill;

  [Header("Pattern")]
  public EnemyPattern pattern;

  [Header("Locomotion")]
  public float BaseSpeed = 3f;
  public float Accel = 12f;
  public float TurnSharpness = 10f;
  public float KnockbackFriction = 20f;

  [Header("Steering")]
  public float moveSpeed = 3f;
  public float turnSpeed = 10f;

  [Header("Avoidance")]
  public float obstacleDist = 0.9f;
  public float obstacleStrength = 1.4f;
  public float avoidAngle = 40f;

  [Header("Separation")]
  public float separationRadius = 0.9f;
  public float separationStrength = 1.2f;
  public LayerMask playerMask;
  public LayerMask obstacleMask;
  public LayerMask enemyLayer;

  [Header("Corner / Narrow Passage")]
  public float cornerRadius = 0.55f;
  public float cornerPush = 1.8f;
  public float passageProbeDist = 1.2f;
  public float narrowThreshold = 0.95f;
  public float centerStrength = 1.6f;
  public float narrowSpeedMul = 0.6f;
  
}