
public class MockInstaller
{
  public void Install(DIContainerBase container, GameSceneInstaller scene)
  {
    scene.MockTest.Initialize(container.Get<WorldZoneManager>());
    scene.SpawnMock.Initialze(scene.EnemySpawner);
  }
}