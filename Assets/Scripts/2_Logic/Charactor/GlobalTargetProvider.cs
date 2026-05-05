using UnityEngine;

public class GlobalTargetProvider : MonoBehaviour
{
  public Transform Player;
  public Transform Base;
  public static GlobalTargetProvider Instance { get; private set; }

  private void Awake()
  {
    if (Instance != null && Instance != this) Destroy(gameObject);
    Instance = this;
  }
}