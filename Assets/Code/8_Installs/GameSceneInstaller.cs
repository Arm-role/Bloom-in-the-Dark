using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class GameSceneInstaller : SceneInstaller
{
    [Header("GameSetting")]
    public GameSceneSettings GameSetting;
    public MockSettings MockSettings;
    public UIManager UIManager;

    [Header("Input")]
    public InputReader InputRender;
    public DragDropController DragDropController;

    [Header("Player")]
    public Transform PlayerPivot;
    public PlayerController PlayerController;

    [Header("Inventory")]
    public HotbarController HotbarController;
    public InventoryController InventoryController;
    public InventoryView HotbarInventoryView;
    public InventoryView MainInventoryView;
    public DragGhost DragGhost;

    [Header("Grid")]
    public PreviewGridView PreviewGridView;
    public AreaCircleIndicatorPreview AreaCirclePreview;
    public PlacementPreviewController PlacementPreviewController;

    [Header("TurnSystem")]
    [SerializeField] public TurnSystem TurnSystem;

    [Header("Tilemaps")]
    public WorldTileManager WorldTileManager;
    public Tilemap MainTilemap;
    public List<TilemapLayer> TilemapLayers;

    [Header("Mock")]
    public SpawnMock SpawnMock;

    protected override void Initialize(DIContainerBase global)
    {
        var container = new DIContainerBase(global);

        new RuntimeInstaller().Install(container, this);
        new SceneBindingInstaller().Install(container, this);

#if UNITY_EDITOR
        new MockInstaller().Install(container, this);
#endif

        Destroy(gameObject);
    }
}
