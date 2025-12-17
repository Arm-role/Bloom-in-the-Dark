public class TilemapInstaller
{
    public void Install(DIContainerBase container, GameSceneInstaller scene)
    {
        var tilemapService = new TilemapService(
            scene.TilemapLayers,
            container.Get<GridConverter>(),
            scene.GameSetting.TileLibrary,
            scene.WorldTileManager
        );

        var factory = new TileInteractableFactory(
            scene.GameSetting,
            tilemapService,
            container.Get<SpawnerHandle>(),
            container.Get<ParticalService>()
        );

        container.Register(tilemapService);
        container.Register(factory);

        scene.WorldTileManager.Initialize(
            scene.TilemapLayers,
            scene.GameSetting.TileLibrary,
            container.Get<GridConverter>(),
            factory
        );
    }
}
