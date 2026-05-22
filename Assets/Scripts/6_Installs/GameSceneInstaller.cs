using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameSceneInstaller : SceneInstaller
{
  [Header("GameSetting")]
  public GameScriptableModules Scriptable;
  public GamePlaySettings GameMonoSetting;
  public MockSettings MockSettings;
  public InventoryUIRoot InventoryUI;

  [Header("Input")]
  public InputReader InputRender;
  public UIHoverResolver UIHoverResolver;

  [Header("Player")]
  public Transform PlayerTransform;
  public PlayerController PlayerController;
  public BaseBuildingController BaseBuildingController;

  [Header("Inventory")]
  public HotbarController HotbarController;
  public InventoryView HotbarInventoryView;
  public InventoryView MainInventoryView;
  public DragGhost DragGhost;

  [Header("Upgrade")]
  public AltarController AltarController;
  public UpgradeManagerView UpgradeManagerView;
  public AltarRecipeSuggestionView AltarSuggestionView;
  public ExpManagerView ExpManagerView;
  public ProgressionView ProgressionView;

  [Header("Grid")]
  public PreviewGridView PreviewGridView;
  public AreaCircleIndicatorPreview AreaCirclePreview;
  public ConeIndicatorPreview ConePreview;
  public AreaLineIndicatorPreview AreaLinePreview;
  public PlacementPreviewController PlacementPreviewController;

  [Header("Tooltip")]
  public TooltipView TooltipView;

  [Header("TurnSystem")]
  public TurnSystem TurnSystem;
  public TurnView TurnView;

  [Header("Tilemaps")]
  public WorldTileManager WorldTileManager;
  public Tilemap MainTilemap;
  public List<TilemapLayer> TilemapLayers;


  [Header("Enemies")]
  public EntitySpawner EnemySpawner;
  public CycleController CycleController;

  [Header("VFX")]
  public VFXController VFXController;

  [Header("Mock")]
  public SpawnMock SpawnMock;

  [Header("Menu")]
  public PauseMenuController PauseMenuController;

  [Header("Trade")]
  public TradeView TradeView;

  protected override void Initialize(DIContainerBase global)
  {
    var container = new DIContainerBase(global);

    // เข้า scene GamePlay ใหม่ — ล้าง persistent singleton ที่ค้างจาก session ก่อน
    // ก่อน installer populate รอบใหม่ (state machine/pool เป็น DDOL อยู่ถาวร)
    global.Get<IAdressablePoolService<GameObject>>().ReturnAll();
    global.Get<GameStateMachine>().ResetForNewScene();
    Time.timeScale = 1f; // เผื่อ session ก่อนจบตอนอยู่ UpgradeState (timeScale=0)

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
