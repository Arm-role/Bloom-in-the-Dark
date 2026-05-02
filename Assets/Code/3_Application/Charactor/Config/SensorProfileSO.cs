using UnityEngine;

[CreateAssetMenu(menuName = "AI/Sensor Profile")]
public class SensorProfileSO : ScriptableObject
{
  [Tooltip("ระยะ detect ขั้นต่ำ ไม่ว่า attack range จะเป็นเท่าไหร่")]
  public float MinDetectionRadius = 5f;

  [Tooltip("ระยะ chase ขั้นต่ำ")]
  public float MinChaseRadius = 8f;

  public float detectionMultiplier = 1.5f;
  public float chaseMultiplier = 2.5f;
}
