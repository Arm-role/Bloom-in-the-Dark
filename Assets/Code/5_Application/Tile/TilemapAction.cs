using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapAction
{
    private readonly TilemapLogic _logic;

    public TilemapAction(TilemapLogic logic)
    {
        _logic = logic;
    }

    public void PlaceTile(Vector3Int cellPos, TileBase tile)
    {
        Debug.Log("PlaceTile");
        if (_logic.CanPlace(cellPos))
            _logic.SetTile(cellPos, tile);
    }

    public void RemoveTile(Vector3Int cellPos)
    {
        Debug.Log("RemoveTile");
        _logic.ClearTile(cellPos);
    }
}