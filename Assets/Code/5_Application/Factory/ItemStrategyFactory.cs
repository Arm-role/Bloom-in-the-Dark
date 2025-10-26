using UnityEngine.Tilemaps;

public class ItemStrategyFactory
{
    private Tilemap _tilemap;
    private GridLogic _gridLogic;

    public ItemStrategyFactory(Tilemap tilemap, GridLogic gridLogic)
    {
        _tilemap = tilemap;
        _gridLogic = gridLogic;
    }

    public ItemStrategyBundle CreateHoe()
    {
        return new ItemStrategyBundle();
    }
}
