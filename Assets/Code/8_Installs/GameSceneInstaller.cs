using UnityEngine;

public class GameSceneInstaller : SceneInstaller
{
    [Header("GameSetting")]
    [SerializeField] private GameSceneSettings _gameSetting;
    [SerializeField] private InteractionService _interactionService;

    [Header("Input")]
    [SerializeField] private InputReader _playerInput;
    [SerializeField] private GameplayInputHandler _playerInputHandler;

    [Header("Player")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private ItemActionController _itemActionController;

    [Header("Inventory")]
    [SerializeField] private HotbarController _hotbarController;
    [SerializeField] private InventoryController _inventoryController;
    [SerializeField] private InventoryView _hotbarInventoryView;
    [SerializeField] private InventoryView _mainInventoryView;

    [Header("Data")]
    [SerializeField] private Item itme1;
    [SerializeField] private Item itme2;

    [Header("Grid")]
    [SerializeField] private PreviewGridView previewGridView;
    [SerializeField] private PlacementController placementController;
    [SerializeField] private PlacementPreviewController placementPreviewController;

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

        //----Aplication Layer---//
        var hotbarState = new HotbarState(_gameSetting.HotbarSize);
        var playerInventory = new PlayerInventory(hotbarState, _gameSetting.HotbarSize, _gameSetting.InventorySize);

        //----Controller Layer---//
        var spawnerHandle = new SpawnerHandle(gameObjectSpawner);
        var InitialzeObject = new GameObjectInitialz(spawnerHandle);

        var inputManager = new InputManager();

        _playerController.Initialze(_gameSetting.MoveSpeed);

        _hotbarController.Initialize(_playerInput);
        _itemActionController.Initialze(_interactionService, playerInventory, particalService);

        _inventoryController.Initialize(_hotbarInventoryView, _hotbarController, playerInventory);
        _inventoryController.MockInstall(itme1, itme2);

        placementPreviewController.Initialze(previewGridView);
        placementController.Initialze(_playerInput, placementPreviewController);

        _playerInputHandler.Initialize(_playerInput, inputManager, _playerController, _itemActionController);

        Destroy(gameObject);
    }
}
