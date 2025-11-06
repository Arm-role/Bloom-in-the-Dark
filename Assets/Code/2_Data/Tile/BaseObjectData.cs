using UnityEngine;

[CreateAssetMenu(menuName = "Object/BaseObjectData")]
public class BaseObjectData : ScriptableObject
{
    public string Id;                    // เช่น "Plant", "Totem"
    public GameObject Prefab;
    public Vector2Int Size = Vector2Int.one;
    public string[] PlaceableOnTiles;    // เช่น ["Grass", "Dirt"]
    public bool RequiresFlatGround = true;
    public bool BlocksOtherObjects = true;

    public bool CanPlaceOn(BaseTileData tile)
    {
        if (tile == null) return false;
        foreach (var t in PlaceableOnTiles)
            if (t == tile.DisplayName) return true;
        return false;
    }
}