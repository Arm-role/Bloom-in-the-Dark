using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tile/TileBaseData")]
public class TileBaseData : ScriptableObject, IBaseTileData
{
    [Header("Tile")]
    [SerializeField] private TileBase tile;
    [SerializeField] private string displayName;
    [SerializeField] private ETileLayerType tileLayerType;
    [SerializeField] private ETileType tileType;
    [SerializeField] private EWorldInteractableType worldInteractableType;

    public TileBase Tile => tile;
    public string DisplayName => displayName;
    public ETileLayerType TileLayerType => tileLayerType;
    public ETileType TileType => tileType;
    public EWorldInteractableType WorldInteractableType => worldInteractableType;
}
