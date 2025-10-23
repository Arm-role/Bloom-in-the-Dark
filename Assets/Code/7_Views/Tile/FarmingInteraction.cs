using UnityEngine;
using UnityEngine.Tilemaps;

public class FarmingInteraction : MonoBehaviour
{
    [SerializeField] private Tilemap soilMap;
    [SerializeField] private TileBase plowedSoilTile;

    public void TryPlow(Vector3 worldPosition)
    {
        Vector3Int cellPos = soilMap.WorldToCell(worldPosition);
        var tile = soilMap.GetTile(cellPos);

        if (tile == null)
        {
            soilMap.SetTile(cellPos, plowedSoilTile);
            Debug.Log($"Plowed at {cellPos}");
        }
    }
}