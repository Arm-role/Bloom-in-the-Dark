
using UnityEngine;

public sealed class GridTargetConfigProvider
    : ITargetingConfigProvider
{
  public ITargetingConfig Create(
      InteractionHandleContext ctx)
  {
    var itemInstance = ctx.ItemInstance;
    if (itemInstance == null)
    {
      Debug.LogError("itemInstance");
      return null;
    }

    var data = itemInstance.Data;

    if (data == null)
    {
      Debug.LogError("data");
      return null;
    }

    if (data.InteractionProfile == null)
    {
      Debug.LogError("InteractionProfile");
      return null;
    }

    if (data.PlacementProfile == null)
    {
      Debug.LogError("PlacementProfile");
      return null;
    }

    return new GridTargetConfig(
        data.PlacementProfile.GridSize,
        data.InteractionProfile.Range);
  }
}