using System.Threading.Tasks;

public class MelonBomberAction : IPrimaryAction
{
    public PrimaryActionExecutionResult Execute(IItemInstance interactable)
    {
        var action = new PrimaryActionExecutionResult()
        {
            ShouldSpawnSelf = Task.FromResult(true),
            ParticleToPlay = "BombCircle"
        };

        return action;
    }
}