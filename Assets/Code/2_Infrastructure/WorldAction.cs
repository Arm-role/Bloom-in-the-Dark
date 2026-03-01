using System.Collections.Generic;

public class WorldAction
{
  public string PlaceObject;
  public bool RemoveObject;

  public IBaseTileData AddTile;
  public ETileLayerType TileTargetLayer;
  public bool RemoveTile;

  public List<ItemStack> ItemRewards = new();

  public float DamageTarget;
  public ERewardCondition RewardCondition = ERewardCondition.Immediate;
}

public enum ERewardCondition
{
  Immediate, // ได้ทันที
  OnObjectDestroyed, // ได้เมื่อ object ถูกทำลาย
}