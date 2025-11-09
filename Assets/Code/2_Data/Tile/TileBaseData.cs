using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tile/TileBaseData")]
public class TileBaseData : ScriptableObject, IBaseTileData
{
    [Header("Tile")]
    [SerializeField] private TileBase tile;
    [SerializeField] private string displayName;
    [SerializeField] private bool canBeReplaced;
    [SerializeField] private ETileLayerType tileLayerType;
    [SerializeField] private ETileType tileType;
    [SerializeField] private string[] replaceableTiles;

    public TileBase Tile => tile;
    public string DisplayName => displayName;
    public bool CanBeReplaced => canBeReplaced;
    public ETileLayerType TileLayerType => tileLayerType;
    public ETileType TileType => tileType;
    public IReadOnlyList<string> ReplaceableTiles => replaceableTiles;

    public bool CanReplace(TileBaseData target)
    {
        if (target == null) return false;
        foreach (var t in ReplaceableTiles)
            if (t == target.DisplayName) return true;
        return false;
    }
}
