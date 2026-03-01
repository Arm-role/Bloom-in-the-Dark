using System.Linq;
using System.Collections.Generic;

public class CellActionRegistry
{
    private readonly Dictionary<InteractionStage, List<ICellAction>> _byStage
        = new();

    public bool HasAnyInteractable => _byStage.Values.Count > 0;

    public void Register(ICellAction action)
    {
        if (!_byStage.TryGetValue(action.Stage, out var list))
        {
            list = new List<ICellAction>();
            _byStage[action.Stage] = list;
        }

        list.Add(action);
    }
    
    public void Registers(IEnumerable<ICellAction> actionList)
    {
        foreach (var action in actionList.ToList())
        {
            Register(action);
        }
    }

    // ---------- Query ----------
    public IEnumerable<ICellAction> GetByStage(InteractionStage stage)
    {
        if (_byStage.TryGetValue(stage, out var list))
            return list;

        return Enumerable.Empty<ICellAction>();
    }

    public void Clear()
    {
        _byStage.Clear();
    }
}