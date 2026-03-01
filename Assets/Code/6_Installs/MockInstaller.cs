using System.Linq;

public class MockInstaller
{
    public void Install(DIContainerBase container, GameSceneInstaller scene)
    {
        scene.SpawnMock.Initialze(container.Get<SpawnerHandle>());
    }
}