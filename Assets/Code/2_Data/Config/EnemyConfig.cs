using UnityEngine;

[CreateAssetMenu(menuName = "AI/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    [Header("Locomotion")]
    public float baseSpeed = 3f;
    public float accel = 12f;
    public float turnSharpness = 10f;

    [Header("Steering")]
    public float moveSpeed = 3f;
    public float turnSpeed = 10f;

    [Header("Avoidance")]
    public float obstacleDist = 0.9f;
    public float obstacleStrength = 1.4f;
    public float avoidAngle = 40f;
    public LayerMask obstacleMask;

    [Header("Separation")]
    public float separationRadius = 0.9f;
    public float separationStrength = 1.2f;
    public LayerMask enemyLayer;

    [Header("Corner / Narrow Passage")]
    public float cornerRadius = 0.55f;
    public float cornerPush = 1.8f;
    public float passageProbeDist = 1.2f;
    public float narrowThreshold = 0.95f;
    public float centerStrength = 1.6f;
    public float narrowSpeedMul = 0.6f;

    [Header("Sensor")]
    public LayerMask targetMask;
}