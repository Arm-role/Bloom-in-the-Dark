using System.Collections.Generic;

public interface ICellActionProvider
{
    IEnumerable<ICellAction> CreateActions(
        CellActionContext ctx);
}