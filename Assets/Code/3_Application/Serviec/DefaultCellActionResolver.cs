public class DefaultCellActionResolver : ICellActionResolver
{
    private readonly GameActionFactory _factory;

    public DefaultCellActionResolver(GameActionFactory factory)
    {
        _factory = factory;
    }

    public void Resolve(WorldCell cell, ActionRegistry registry)
    {
        foreach (var tile in cell.Tiles)
            registry.Registers(_factory.CreateActions(tile));

        if (cell.Object != null)
            registry.Registers(_factory.CreateActions(cell.Object));
    }
}
