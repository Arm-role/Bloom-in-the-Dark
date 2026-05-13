using UnityEngine;
using UnityEngine.EventSystems;

public class GamePlaySettings : MonoBehaviour
{
  [Header("DRAG CONFIGURATION")]
  public Camera Camera;
  public EventSystem eventSystem;

  void Awake()
  {
    Camera.main.transparencySortMode = TransparencySortMode.CustomAxis;
    Camera.main.transparencySortAxis = new Vector3(0, 1, 0);
  }
}