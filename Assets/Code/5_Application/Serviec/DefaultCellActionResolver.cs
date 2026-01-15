public class DefaultCellActionResolver : ICellActionResolver
{
    private readonly CellInteractableFactory _factory;

    public DefaultCellActionResolver(CellInteractableFactory factory)
    {
        _factory = factory;
    }

    public void Resolve(WorldCell cell, CellActionRegistry registry)
    {
        foreach (var tile in cell.Tiles)
            registry.Registers(_factory.CreateActions(tile));

        if (cell.PlacedObject != null)
            registry.Registers(_factory.CreateActions(cell.PlacedObject));
    }
}