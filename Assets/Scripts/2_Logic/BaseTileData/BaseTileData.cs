using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseTileData
    : ScriptableObject,
        IBaseTileData
{
    [SerializeField] private TileBase[] _tiles;
    [SerializeField] private string _displayName;
    [SerializeField] private ETileLayerType _tileLayerType;
    [SerializeField] private ETargetType _targetType;
    
    public IReadOnlyList<TileBase> Tiles => _tiles;
    public string DisplayName => _displayName;
    public ETileLayerType TileLayerType => _tileLayerType;
    public ETargetType TargetType  => _targetType;
}