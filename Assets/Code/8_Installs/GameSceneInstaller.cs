using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class GameSceneInstaller : SceneInstaller
{
    [Header("GameSetting")]
    public GameSceneSettings GameSetting;
    public MockSettings MockSettings;
    public InteractionService InteractionService;
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
    public PlacementPreviewController PlacementPreviewController;
    public AreaCirclePreview AreaCirclePreview;

    [Header("TurnSystem")]
    [SerializeField] public TurnSystem TurnSystem;

    [Header("Tilemaps")]
    public WorldTileManager WorldTileManager;
    public Tilemap MainTilemap;
    public List<TilemapLayer> TilemapLayers;

    [Header("Mock")]
    public SpawnMock SpawnMock;

    protected override void Initialzed(DIContainerBase global)
    {
        var container = new DIContainerBase(global);

        new CoreInstaller().Install(container, this);
        new PlayerInstaller().Install(container, this);
        new InventoryInstaller().Install(container, this);
        new GridInstaller().Install(container, this);
        new TilemapInstaller().Install(container, this);
        new InteractionInstaller().Install(container, this);

#if UNITY_EDITOR
        new MockInstaller().Install(container, this);
#endif

        Destroy(gameObject);
    }
}
