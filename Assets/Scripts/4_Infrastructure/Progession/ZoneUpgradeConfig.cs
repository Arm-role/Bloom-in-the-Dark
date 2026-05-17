using UnityEngine;

[CreateAssetMenu(fileName = "ZoneUpgradeConfig", menuName = "Game/Zone Upgrade Config")]
public sealed class ZoneUpgradeConfig : ScriptableObject, IZoneUpgradeConfig
{
    [SerializeField] private CellZoneFlags zoneFlags;
    public CellZoneFlags ZoneFlags => zoneFlags;
}
