public readonly struct CellActionContext
{
    public readonly TileLibrary TileLibrary;

    public CellActionContext(
        TileLibrary tileLibrary)
    {
        TileLibrary = tileLibrary;
    }
}