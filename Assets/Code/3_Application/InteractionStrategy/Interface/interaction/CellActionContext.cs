public readonly struct CellActionContext
{
    public readonly ITileLibrary TileLibrary;

    public CellActionContext(
        ITileLibrary tileLibrary)
    {
        TileLibrary = tileLibrary;
    }
}