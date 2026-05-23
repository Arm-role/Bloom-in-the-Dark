#nullable enable
using UnityEngine;

public sealed class WorldHoverResolver : IHoverResolver
{
  private readonly Camera _camera;
  private readonly LayerMask _layerMask;

  public WorldHoverResolver(LayerMask layerMask, Camera camera)
  {
    _layerMask = layerMask;
    _camera = camera;
  }

  public HoverState Resolve(Vector2 pointerScreenPosition)
  {
    Vector3 world = _camera.ScreenToWorldPoint(pointerScreenPosition);
    Vector2 worldPoint = new Vector2(world.x, world.y);

    Collider2D hit = Physics2D.OverlapPoint(worldPoint, _layerMask);

    return hit != null ? HoverState.World : HoverState.None;
  }
}
