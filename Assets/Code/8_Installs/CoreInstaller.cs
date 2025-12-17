using UnityEngine;

public class CoreInstaller
{
    public void Install(DIContainerBase container, GameSceneInstaller scene)
    {
        var pool = container.Get<IAdressablePoolService<GameObject>>();

        var spawner = new GameObjectSpawner(pool, scene.GameSetting.GameObjectLibrary);
        var spawnerHandle = new SpawnerHandle(spawner);
        var particle = new ParticalService(pool, scene.GameSetting.ParticleLibrary);

        container.Register(spawner);
        container.Register(spawnerHandle);
        container.Register(particle);

        var gameApplication = container.Get<GameApplication>();
        gameApplication.Initialize(scene.InputRender);
    }
}
