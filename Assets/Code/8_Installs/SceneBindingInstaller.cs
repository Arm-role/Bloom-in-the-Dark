public class SceneBindingInstaller
{
    public void Install(DIContainerBase container, GameSceneInstaller scene)
    {
        var gameApplication = container.Get<GameApplication>();

        var state = container.Get<PlayerState>();
        var data = container.Get<PlayerData>();

        var hotbarState = container.Get<HotbarState>();
        var inventory = container.Get<PlayerInventory>();

        var cellResoulver = container.Get<DefaultCellActionResolver>();

        var handleService = container.Get<InteractionHandleService>();
        var playerState = container.Get<PlayerState>();
        var playerInventory = container.Get<PlayerInventory>();
        var particalService = container.Get<ParticalService>();

        gameApplication.Initialize(scene.InputRender);

        // =======================
        // Player
        // =======================

        scene.PlayerController.Initialize(
            scene.InputRender,
            data,
            state,
            playerInventory
        );

        scene.DragDropController.Initialize(
            scene.InputRender,
            scene.GameSetting.holdThreshold,
            scene.GameSetting.holdMoveTolerance
        );

        // =======================
        // Inventory
        // =======================

        var inventoryState = container.Get<InventoryState>();

        inventoryState.Initialize(
            scene.UIManager.OnEnterInventory,
            scene.UIManager.OnExitInventory
        );

        scene.HotbarController.Initialize(scene.InputRender, hotbarState);

        scene.InventoryController.Initialize(
            inventory,
            hotbarState,
            scene.HotbarInventoryView,
            scene.MainInventoryView,
            scene.DragGhost
        );

        // =======================
        // Grid
        // =======================

        scene.PlacementPreviewController.Initialze(scene.PreviewGridView);

        // =======================
        // TileMap
        // =======================
        
        scene.WorldTileManager.Initialize(
            scene.TilemapLayers,
            scene.GameSetting.TileLibrary,
            container.Get<GridConverter>(),
            cellResoulver
        );

        // =======================
        // FlowState
        // =======================

        var gameplayState = container.Get<GameplayState>();

        gameplayState.Initialize(
            scene.PlayerController,
            scene.DragDropController
        );

        var interactionAction = new ItemInteractionAction(
            handleService,
            scene.GameSetting.CostService,
            scene.PlayerPivot,
            scene.PlayerController.Interactor,
            playerState,
            scene.DragDropController,
            particalService);
    }
}