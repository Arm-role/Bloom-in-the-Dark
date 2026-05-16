public sealed class FenceController : BaseBuildingController, IBreakableWall
{
    public bool IsDestroyed => !IsAlive;

    protected override void OnBroken()
    {
        base.OnBroken();
        FlowFieldManager.Instance?.InvalidateAll();
    }
}
