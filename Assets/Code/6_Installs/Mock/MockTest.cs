using UnityEngine;

public class MockTest : MonoBehaviour
{
  [SerializeField] private float radius;
  [SerializeField] private CellZoneFlags zoneFlags;

  private WorldZoneManager _zoneManager;
  public void Initialize(WorldZoneManager zoneManager)
  {
    _zoneManager = zoneManager;

    _zoneManager.ZoneChange(radius, zoneFlags);
  }
  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Z))
    {
      _zoneManager.ZoneChange(radius, zoneFlags);
    }
  }
}