using System.Linq;
using System.Collections.Generic;

public class ActionRegistry
{
  private readonly Dictionary<InteractionStage, List<IGameAction>> _byStage
      = new();

  public bool HasAnyInteractable => _byStage.Values.Count > 0;

  public void Register(IGameAction action)
  {
    if (!_byStage.TryGetValue(action.Stage, out var list))
    {
      list = new List<IGameAction>();
      _byStage[action.Stage] = list;
    }

    list.Add(action);
  }

  public void Registers(IEnumerable<IGameAction> actionList)
  {
    foreach (var action in actionList.ToList())
    {
      Register(action);
    }
  }

  // ---------- Query ----------
  public IEnumerable<IGameAction> GetByStage(InteractionStage stage)
  {
    if (_byStage.TryGetValue(stage, out var list))
      return list;

    return Enumerable.Empty<IGameAction>();
  }

  public void Clear()
  {
    _byStage.Clear();
  }
}