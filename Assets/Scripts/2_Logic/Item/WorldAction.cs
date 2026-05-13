using System.Collections.Generic;
using UnityEngine;

public class WorldAction
{
  public int PlaceObjectId;
  public bool RemoveObject;

  public IBaseTileData AddTile;
  public ETileLayerType TileTargetLayer;
  public bool RemoveTile;

  public float Exp;
  public Vector3 SourcePosition;

  public List<ItemStack> ItemRewards = new();

  public float DamageTarget;
  public ERewardCondition RewardCondition = ERewardCondition.Immediate;
}
public enum ERewardCondition
{
  Immediate, // ได้ทันที
  OnObjectDestroyed, // ได้เมื่อ object ถูกทำลาย
}