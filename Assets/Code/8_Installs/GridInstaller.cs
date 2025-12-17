public class GridInstaller
{
    public void Install(DIContainerBase container, GameSceneInstaller scene)
    {
        var gridConverter = new GridConverter(scene.MainTilemap);

        container.Register(gridConverter);

        scene.PlacementPreviewController.Initialze(scene.PreviewGridView);
    }
}
