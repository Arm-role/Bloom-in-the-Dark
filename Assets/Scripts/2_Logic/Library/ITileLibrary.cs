using UnityEngine.Tilemaps;

public interface ITileLibrary 
{
  public BaseTileData GetTileData(TileBase tile);
  public BaseTileData GetTileBaseDataByName(string tileName);
}