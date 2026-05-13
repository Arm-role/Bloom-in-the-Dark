using UnityEngine;

public class WorldHoverResolver : IHoverResolver
{
  private readonly LayerMask _layerMask;

  public WorldHoverResolver(LayerMask layerMask)
  {
    _layerMask = layerMask;
  }

  public HoverState Resolve(Vector2 pointerScreenPosition)
  {
    Vector3 world = Camera.main.ScreenToWorldPoint(pointerScreenPosition);
    Vector2 worldPoint = new Vector2(world.x, world.y);

    Collider2D hit = Physics2D.OverlapPoint(worldPoint, _layerMask);

    return hit != null ? HoverState.World : HoverState.None;
  }
}