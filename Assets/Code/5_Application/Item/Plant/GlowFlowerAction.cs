using System.Threading.Tasks;

public class GlowFlowerAction : IPrimaryAction
{
    public PrimaryActionExecutionResult Execute(IItemInstance interactable)
    {
        var action = new PrimaryActionExecutionResult()
        {
            ShouldSpawnSelf = Task.FromResult(true),
            ParticleToPlay = "GlowFlower"
        };

        return action;
    }
}
