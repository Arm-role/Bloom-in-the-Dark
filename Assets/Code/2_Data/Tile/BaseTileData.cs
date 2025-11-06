using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tile/BaseTileData")]
public class BaseTileData : ScriptableObject, IBaseTileData
{
    [Header("Tile")]
    [SerializeField] private TileBase tile;
    [SerializeField] private string displayName;
    [SerializeField] private bool canBeReplaced;
    [SerializeField] private string[] replaceableTiles;

    public TileBase Tile => tile;
    public string DisplayName => displayName;
    public bool CanBeReplaced => canBeReplaced;
    public IReadOnlyList<string> ReplaceableTiles => replaceableTiles;

    public bool CanReplace(BaseTileData target)
    {
        if (target == null) return false;
        foreach (var t in ReplaceableTiles)
            if (t == target.DisplayName) return true;
        return false;
    }
}
