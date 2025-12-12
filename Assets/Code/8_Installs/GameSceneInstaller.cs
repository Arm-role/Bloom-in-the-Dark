using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameSceneInstaller : SceneInstaller
{
    [Header("GameSetting")]
    [SerializeField] private GameSceneSettings _gameSetting;
    [SerializeField] private MockSettings _mockSettings;
    [SerializeField] private InteractionService _interactionService;
    [SerializeField] private UIManager _uiManager;

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
    [SerializeField] private DragGhost _dragGhost;

    [Header("Grid")]
    [SerializeField] private PreviewGridView _previewGridView;
    [SerializeField] private PlacementPreviewController _placementPreviewController;
    [SerializeField] private AreaCirclePreview _areaCirclePreview;

    [Header("TurnSystem")]
    [SerializeField] private TurnSystem _turnSystem;

    [Header("Tilemaps")]
    [SerializeField] private WorldTileManager _worldTileManager;
    [SerializeField] private Tilemap _mainTilemap;
    [SerializeField] private LayerMask _layerMask;

    [SerializeField] private List<TilemapLayer> _tilemapLayers = new();

    [Header("Mock")]
    [SerializeField] private SpawnMock _spawnMock;

    protected override void Start() => base.Start();
    protected override void OnDestroy() => base.OnDestroy();

    protected override void Initialzed(DIContainerBase globalContainer)
    {
        AppInstaller.OnServiceReady -= Initialzed;

        var container = new DIContainerBase(globalContainer);

        //----Abstraction Layer---//
        var poolService = container.GetObject<IAdressablePoolService<GameObject>>();
        var gameObjectSpawner = new GameObjectSpawner(poolService, _gameSetting.GameObjectLibrary);
        var spawnerHandle = new SpawnerHandle(gameObjectSpawner);
        var particalService = new ParticalService(poolService, _gameSetting.ParticleLibrary);

        var playerState = new PlayerState(FacingDirection.Right);
        var playerData = new PlayerData(_gameSetting.MaxHP);
        var playerEnergy = new PlayerEnergy(_gameSetting.MaxEnergy);

        //----Inventory---//
        var hotbarState = new HotbarState(_gameSetting.HotbarSize);
        var playerInventory = new PlayerInventory(
            hotbarState,
            _gameSetting.HotbarSize,
            _gameSetting.InventorySize);

        //----Controller Layer---//
        var worldInteractionExecutor = new WorldInteractionExecutor(
            _gameSetting.ItemsLibrary,
            spawnerHandle,
            playerInventory);

        var initialzeObject = new GameObjectInitialz(
            _turnSystem,
            spawnerHandle,
            _worldTileManager,
            worldInteractionExecutor);

        var inputManager = new InputManager();

        //----Grid---//
        var worldGrid = new WorldGridLogic();
        var gridConverter = new GridConverter(_mainTilemap);

        var interactionHandleService = new InteractionHandleService();

        var interactionTargetResolver = new InteractionTargetResolver(
            gridConverter,
            _worldTileManager);

        var interactionDispatcher = new InteractionDispatcher(
            _gameSetting.DefaultLayerPriorityRule,
            interactionTargetResolver);

        var tilemapService = new TilemapService(
            _tilemapLayers,
            gridConverter,
            _gameSetting.TileLibrary,
            _worldTileManager);

        var tileInteractableFactory = new TileInteractableFactory(
            _gameSetting,
            tilemapService,
            spawnerHandle,
            particalService);

        var itemStrategyFactory = new ItemStrategyFactory(
            _gameSetting,
            gridConverter,
            _placementPreviewController,
            _areaCirclePreview,
            interactionDispatcher,
            tilemapService,
            _worldTileManager,
            gameObjectSpawner,
            particalService);

        var gridBaseBudle = itemStrategyFactory.CreateGridBasedStategyBundle();
        var gridTargetingBudle = itemStrategyFactory.CreateGridTargetingStategyBundle();
        var proximityBudle = itemStrategyFactory.CreateProximityColliderStategyBundle();
        var areaCircleBudle = itemStrategyFactory.CreateAreaCircleStategyBundle();
        var areaLien = itemStrategyFactory.CreateLineAttackStrategy();
        var directInteractBudle = itemStrategyFactory.CreateDirectInteractStategyBundle();

        interactionHandleService.Register(EItemStrategyType.GridBased, gridBaseBudle);
        interactionHandleService.Register(EItemStrategyType.GridTargeting, gridTargetingBudle);
        interactionHandleService.Register(EItemStrategyType.ProximityCollider, proximityBudle);
        interactionHandleService.Register(EItemStrategyType.AreaCircle, areaCircleBudle);
        interactionHandleService.Register(EItemStrategyType.AreaLine, areaLien);

        interactionHandleService.Register(EItemStrategyType.DirectInteract, directInteractBudle);

        var itemInteractionAction = new ItemInteractionAction(
            interactionHandleService,
            _interactionService,
            _playerPivot,
            playerState,
            playerEnergy,
            _dragDropController,
            playerInventory,
            particalService);

        _dragDropController.Initialze(_gameSetting.holdThreshold, _gameSetting.holdMoveTolerance);

        _playerController.Initialze(_gameSetting.MoveSpeed, playerData, playerState, playerEnergy);

        _hotbarController.Initialize(_playerInput);

        _inventoryController.Initialize(_hotbarInventoryView, _mainInventoryView, _dragGhost, _hotbarController, playerInventory);

        _placementPreviewController.Initialze(_previewGridView);

        _playerInputHandler.Initialize(_playerInput, inputManager, _playerController, _dragDropController);

        _worldTileManager.Initialize(_tilemapLayers, _gameSetting.TileLibrary, gridConverter, tileInteractableFactory);

        _uiManager.Initialze(_playerInput);

        List<IItemData> itemsList = _mockSettings.items.Cast<IItemData>().ToList();

        _inventoryController.MockInstall(itemsList);

        _spawnMock.Initialze(spawnerHandle);

        Destroy(gameObject);
    }
}