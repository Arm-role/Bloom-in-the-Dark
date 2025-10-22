using System;
using UnityEngine;

[Serializable] 
public struct WorldTileData
{
    public Vector2Int Position;
    public string TileID; 

}
public interface IWorldGridModel
{
    Action<WorldTileData> OnTilePlaced { get; set; }
    Action<Vector2Int> OnTileRemoved { get; set; }
}
