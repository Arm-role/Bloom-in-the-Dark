
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New SoilTile Data", menuName = "Tiles/Soil Tile")]
public class SoilTile : Tile
{
    public bool IsWet;

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }
}