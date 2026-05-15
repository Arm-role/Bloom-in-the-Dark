using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverResolver : MonoBehaviour, IHoverResolver
{
    [SerializeField] private List<RectTransform> _blockedAreas = new();

    public HoverState Resolve(Vector2 screenPosition)
    {
        foreach (var rect in _blockedAreas)
        {
            if (rect == null || !rect.gameObject.activeInHierarchy)
                continue;

            if (RectTransformUtility.RectangleContainsScreenPoint(rect, screenPosition, null))
                return HoverState.UI;
        }

        return HoverState.None;
    }
}

[System.Flags]
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
