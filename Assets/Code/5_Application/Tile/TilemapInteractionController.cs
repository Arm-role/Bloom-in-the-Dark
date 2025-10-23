using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapInteractionController 
{
    private Tilemap mainTilemap;
    private TileBase placeTile;

    private TilemapLogic _logic;
    private TilemapAction _action;

    public void Initial(Tilemap tilemap, TileBase tilebase)
    {
        mainTilemap = tilemap;
        placeTile = tilebase;
    }
    public void SetUp()
    {
        _logic = new TilemapLogic(mainTilemap);
        _action = new TilemapAction(_logic);
    }

    public void PlaceTile(Vector3 worldPos)
    {
        Vector3Int cellPos = mainTilemap.WorldToCell(worldPos);
        _action.PlaceTile(cellPos, placeTile);
    }
    public void RemoveTile(Vector3 worldPos)
    {
        Vector3Int cellPos = mainTilemap.WorldToCell(worldPos);
        _action.RemoveTile(cellPos);
    }
}
