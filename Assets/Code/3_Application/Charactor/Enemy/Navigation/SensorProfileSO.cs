using UnityEngine;

[CreateAssetMenu(menuName = "AI/Sensor Profile")]
public class SensorProfileSO : ScriptableObject
{
  public float detectionMultiplier = 1.5f;
  public float chaseMultiplier = 2.5f;
}
