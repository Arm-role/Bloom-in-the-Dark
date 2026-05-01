using UnityEngine;

[CreateAssetMenu(menuName = "AI/Target Selector Profile")]
public class TargetSelectorProfileSO : ScriptableObject
{
  public bool AggroOnSpawn = false;
  public float SpawnThreat = 2f;

  public float DistanceWeight = 1f;
  public float ThreatWeight = 1f;
  public float ObjectiveWeight = 5f;
  public float PlayerWeight = 1f;

  [Header("Aggro")]
  [Tooltip("ความเร็วที่ threat decay ต่อวินาที — boss ควรต่ำ (จำแค้นนาน)")]
  public float ThreatDecayRate = 0.5f;

  [Tooltip("threat ขั้นต่ำที่ยังคง aggro ไว้ก่อน decay ออก")]
  public float MinAggroThreshold = 0f;
}