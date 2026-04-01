using UnityEngine;

public class GrassVelocityController : MonoBehaviour
{
  [Header("Influence Settings")]
  [Range(0f, 2f)]
  public float ExternInfluenceStrength = 0.5f;

  public float InfluenceRadius = 0.7f;
  public float VelocityThreshold = 1f;

  [Header("Smoothing")]
  [Tooltip("Higher = snap faster")]
  public float EaseSpeed = 8f;

  [Header("Optional Boost")]
  public float ImpactMultiplier = 1.5f;
  public float ImpactVelocityThreshold = 3f;

  public int ExternInfluenceID { get; private set; }

  private void Awake()
  {
    ExternInfluenceID = Shader.PropertyToID("_ExternInfluence");
  }
}