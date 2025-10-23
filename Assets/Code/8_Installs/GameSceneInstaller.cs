using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameSceneInstaller : SceneInstaller
{
    [Header("GameSetting")]
    [SerializeField] private GameSceneSettings _gameSetting;
    [SerializeField] private InteractionService _interactionService;

    [Header("Input")]
    [SerializeField] private InputReader _playerInput;
    [SerializeField] private GameplayInputHandler _playerInputHandler;
    [SerializeField] private DragDropController _dragDropController;

    [Header("Player")]
    [SerializeField] private Transform _playerPivot;
    [SerializeField] private PlayerController _playerController;

    [Header("Inventory")]
    [SerializeField] private HotbarController _hotbarController;
    [SerializeField] private InventoryController _inventoryController;
    [SerializeField] private InventoryView _hotbarInventoryView;
    [SerializeField] private InventoryView _mainInventoryView;

    [Header("Data")]
    [SerializeField] private List<Item> items;

    [Header("Grid")]
    [SerializeField] private PreviewGridView previewGridView;
    [SerializeField] private PlacementController placementController;
    [SerializeField] private PlacementPreviewController placementPreviewController;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap mainTilemap;

    protected override void Start() => base.Start();
    protected override void OnDestroy() => base.OnDestroy();

    protected override void Initialzed(DIContainerBase globalContainer)
    {
        AppInstaller.OnServiceReady -= Initialzed;

        var container = new DIContainerBase(globalContainer);

        //----Abstraction Layer---//
        var poolService = container.GetObject<IAdressablePoolService<GameObject>>();
        var gameObjectSpawner = new GameObjectSpawner(poolService, _gameSetting.GameObjectLibrary);
        var particalService = new ParticalService(poolService, _gameSetting.ParticleLibrary);

        //----Inventory---//
        var hotbarState = new HotbarState(_gameSetting.HotbarSize);
        var playerInventory = new PlayerInventory(hotbarState, _gameSetting.HotbarSize, _gameSetting.InventorySize);

        //----Controller Layer---//
        var spawnerHandle = new SpawnerHandle(gameObjectSpawner);
        var initialzeObject = new GameObjectInitialz(spawnerHandle);

        var inputManager = new InputManager();


        var previewModeLibrary = new InteractionHandleService();

        //----Grid---//
        var worldGrid = new WorldGridLogic();

        var tilemapInteractionControl = new TilemapInteractionController();
        tilemapInteractionControl.Initial(mainTilemap, _gameSetting.RuleTileLibrary.Find("Soil"));

        var tilemapPlacementAction = new TilemapPlacementAction(tilemapInteractionControl);

        var gridBaseInteractionHandler = new GridBaseInteractionHandler(placementController);
        var toolPreviewHandler = new ToolInteractionHandler(placementController);


        var itemInteractionAction = new ItemInteractionAction(previewModeLibrary, _playerPivot, _dragDropController, _interactionService, playerInventory, particalService);

        previewModeLibrary.Register(EItemType.Building, EItemStategyType.GridBased, gridBaseInteractionHandler, tilemapPlacementAction);
        previewModeLibrary.Register(EItemType.Tool, EItemStategyType.GridBased, toolPreviewHandler, tilemapPlacementAction);

        _dragDropController.Initialze(_gameSetting.holdThreshold, _gameSetting.holdMoveTolerance);

        _playerController.Initialze(_gameSetting.MoveSpeed);

        _hotbarController.Initialize(_playerInput);

        _inventoryController.Initialize(_hotbarInventoryView, _hotbarController, playerInventory);

        List<IItemData> itemsList = items.Cast<IItemData>().ToList();
        _inventoryController.MockInstall(itemsList);

        placementPreviewController.Initialze(previewGridView);
        placementController.Initialze(placementPreviewController, worldGrid);

        _playerInputHandler.Initialize(_playerInput, inputManager, _playerController, _dragDropController);


        Destroy(gameObject);
    }
}