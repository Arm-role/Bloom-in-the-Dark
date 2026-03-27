using System.Collections.Generic;

public interface ICellActionFactory
{
    IEnumerable<ICellAction> CreateActions(
        BaseTileData baseTileData,
        WorldCell cell);
}