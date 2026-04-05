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
}