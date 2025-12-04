using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tile/TileBaseData")]
public class TileBaseData : ScriptableObject, IBaseTileData
{
    [Header("Tile")]
    [SerializeField] private TileBase[] tiles;
    [SerializeField] private string displayName;
    [SerializeField] private ETileLayerType tileLayerType;
    [SerializeField] private ETileType tileType;
    [SerializeField] private ETileBlockType worldInteractableType;

    public IReadOnlyList<TileBase> Tiles => tiles;
    public string DisplayName => displayName;
    public ETileLayerType TileLayerType => tileLayerType;
    public ETileType TileType => tileType;
    public ETileBlockType WorldInteractableType => worldInteractableType;
}