using UnityEngine;

[CreateAssetMenu(menuName = "AI/Steering Profile")]
public class SteeringProfileSO : ScriptableObject
{
  public float moveSpeed = 3.2f;
  public float turnSpeed = 6.5f;
  public float accel = 14f;

  public float obstacleDist = 0.7f;
  public float obstacleStrength = 1.8f;
  public float avoidAngle = 60f;

  public float separationRadius = 0.9f;
  public float separationStrength = 1.2f;

  public float cornerPush = 0.9f;

  public float narrowThreshold = 1.4f;
  public float narrowSpeedMul = 0.6f;
}
