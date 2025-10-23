
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Tile Data", menuName = "Game/Tile Data")]
public class TileData : ScriptableObject
{
    public string ID;

    public TileBase DisplayTile;

    // public bool IsWalkable;
    // public bool NeedsWater;
}
