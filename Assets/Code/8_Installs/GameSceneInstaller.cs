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
    [SerializeField] private PreviewGridView _previewGridView;
    [SerializeField] private PlacementPreviewController _placementPreviewController;
    [SerializeField] private AreaCirclePreview _areaCirclePreview;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap _mainTilemap;
    [SerializeField] private LayerMask _layerMask;

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

        var playerData = new PlayerData();

        //----Inventory---//
        var hotbarState = new HotbarState(_gameSetting.HotbarSize);
        var playerInventory = new PlayerInventory(
            hotbarState,
            _gameSetting.HotbarSize,
            _gameSetting.InventorySize);

        //----Controller Layer---//
        var spawnerHandle = new SpawnerHandle(gameObjectSpawner);
        var initialzeObject = new GameObjectInitialz(spawnerHandle);

        var inputManager = new InputManager();

        var interactionHandleService = new InteractionHandleService();

        //----Grid---//
        var worldGrid = new WorldGridLogic();
        var gridConverter = new GridConverter(_mainTilemap);
        var worldTileState = new WorldTileState();

        var interactionTargetResolver = new InteractionTargetResolver(
            _layerMask,
            _mainTilemap,
            _gameSetting.TileLibrary, 
            gridConverter);

        var itemInteractionAction = new ItemInteractionAction(
            interactionHandleService,
            _interactionService,
            interactionTargetResolver,
            _playerPivot,
            playerData,
            _dragDropController,
            playerInventory,
            particalService);

        var itemStrategyFactory = new ItemStrategyFactory(
            _gameSetting,
            _mainTilemap,
            gridConverter,
            worldTileState,
            _placementPreviewController,
            _areaCirclePreview,
            interactionTargetResolver,
            gameObjectSpawner,
            particalService);

        var gridBaseBudle = itemStrategyFactory.CreateGridBasedStategyBundle();
        var gridTargetingBudle = itemStrategyFactory.CreateGridTargetingStategyBundle();
        var proximityBudle = itemStrategyFactory.CreateProximityColliderStategyBundle();
        var areaCircleBudle = itemStrategyFactory.CreateAreaCircleStategyBundle();
        var directInteractBudle = itemStrategyFactory.CreateDirectInteractStategyBundle();

        interactionHandleService.Register(EItemType.Building, EItemStategyType.GridBased, gridBaseBudle);
        interactionHandleService.Register(EItemType.Tool, EItemStategyType.GridTargeting, gridTargetingBudle);
        interactionHandleService.Register(EItemType.Tool, EItemStategyType.ProximityCollider, proximityBudle);
        interactionHandleService.Register(EItemType.Plant, EItemStategyType.AreaCircle, areaCircleBudle);
        interactionHandleService.Register(EItemType.Seed, EItemStategyType.DirectInteract, directInteractBudle);

        _dragDropController.Initialze(_gameSetting.holdThreshold, _gameSetting.holdMoveTolerance);

        _playerController.Initialze(_gameSetting.MoveSpeed, playerData);

        _hotbarController.Initialize(_playerInput);

        _inventoryController.Initialize(_hotbarInventoryView, _hotbarController, playerInventory);

        List<IItemData> itemsList = items.Cast<IItemData>().ToList();

        _inventoryController.MockInstall(itemsList);

        _placementPreviewController.Initialze(_previewGridView);

        _playerInputHandler.Initialize(_playerInput, inputManager, _playerController, _dragDropController);

        Destroy(gameObject);
    }
}