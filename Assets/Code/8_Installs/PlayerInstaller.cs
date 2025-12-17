public class PlayerInstaller
{
    public void Install(DIContainerBase container, GameSceneInstaller scene)
    {
        var state = new PlayerState(FacingDirection.Right);
        var data = new PlayerData(scene.GameSetting.MaxHP);
        var energy = new PlayerEnergy(scene.GameSetting.MaxEnergy);

        container.Register(state);
        container.Register(data);
        container.Register(energy);

        scene.PlayerController.Initialze(
            scene.InputRender,
            scene.GameSetting.MoveSpeed,
            data,
            state,
            energy
        );

        scene.DragDropController.Initialze(
            scene.InputRender,
            scene.GameSetting.holdThreshold,
            scene.GameSetting.holdMoveTolerance
        );
    }
}
