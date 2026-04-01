using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class GameSceneInstaller : SceneInstaller
{
  [Header("GameSetting")]
  public GameScriptableModules Scriptable;
  public GamePlaySettings GameMonoSetting;
  public MockSettings MockSettings;
  public InventoryUIRoot InventoryUI;

  [Header("Input")]
  public InputReader InputRender;

  [Header("Player")]
  public Transform PlayerPivot;
  public PlayerController PlayerController;

  [Header("Inventory")]
  public HotbarController HotbarController;
  public InventoryView HotbarInventoryView;
  public InventoryView MainInventoryView;
  public DragGhost DragGhost;

  [Header("Upgrade")]
  public UpgradeUsageLookup UsageLookup;
  public UpgradeManagerView UpgradeManagerView;
  public UpgradeRequestView UpgradeRequestView;
  public ExpManagerView ExpManagerView;

  [Header("Grid")]
  public PreviewGridView PreviewGridView;
  public AreaCircleIndicatorPreview AreaCirclePreview;
  public PlacementPreviewController PlacementPreviewController;

  [Header("TurnSystem")]
  public TurnSystem TurnSystem;
  public TurnView TurnView;

  [Header("Tilemaps")]
  public WorldTileManager WorldTileManager;
  public Tilemap MainTilemap;
  public List<TilemapLayer> TilemapLayers;

  [Header("Enemies")]
  public EnemySpawner EnemySpawner;
  public CycleController CycleController;

  [Header("VFX")]
  public VFXController VFXController;

  [Header("Mock")]
  public MockTest MockTest;
  public SpawnMock SpawnMock;

  protected override void Initialize(DIContainerBase global)
  {
    var container = new DIContainerBase(global);

    new RuntimeInstaller().Install(container, this);
    new SceneBindingInstaller().Install(container, this);

#if UNITY_EDITOR
    new MockInstaller().Install(container, this);
#endif

    var bootstep = container.Get<GameBootstrap>();
    bootstep.StartGame();

    Destroy(gameObject);
  }
}
