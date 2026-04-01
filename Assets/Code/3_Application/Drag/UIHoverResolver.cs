using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIHoverResolver : IHoverResolver
{
  private readonly EventSystem _eventSystem;
  private readonly List<RaycastResult> _results = new();

  public UIHoverResolver(EventSystem eventSystem)
  {
    _eventSystem = eventSystem;
  }

  public HoverState Resolve(Vector2 pointerScreenPosition)
  {
    var data = new PointerEventData(_eventSystem)
    {
      position = pointerScreenPosition
    };

    _results.Clear();
    _eventSystem.RaycastAll(data, _results);

    return _results.Count > 0 ? HoverState.UI : HoverState.None;
  }
}

[Flags]
public enum HoverState
{
  None = 0,
  UI = 1 << 0,
  World = 1 << 1,
}

public interface IHoverResolver
{
  HoverState Resolve(Vector2 pointerScreenPosition);
}
